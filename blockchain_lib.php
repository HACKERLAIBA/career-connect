<?php
/**
 * Minimal blockchain ledger stored in MySQL.
 * - Each block stores a JSON payload and hashes (sha256) to link to previous block.
 * - This is a tamper-evident audit log, not a decentralized blockchain.
 */

function bcHashBlockFields(int $height, string $prevHash, string $dataJson, string $createdAt, int $nonce): string {
    $input = $height . '|' . $prevHash . '|' . $createdAt . '|' . $nonce . '|' . $dataJson;
    return hash('sha256', $input);
}

function bcEnsureTable(mysqli $conn): void {
    $sql = "
        CREATE TABLE IF NOT EXISTS blockchain_blocks (
            id INT AUTO_INCREMENT PRIMARY KEY,
            height INT NOT NULL UNIQUE,
            prev_hash CHAR(64) NOT NULL,
            hash CHAR(64) NOT NULL UNIQUE,
            nonce INT NOT NULL DEFAULT 0,
            data_json LONGTEXT NOT NULL,
            created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
    ";
    $conn->query($sql);
}

function bcGetLatestBlock(mysqli $conn): ?array {
    $res = $conn->query("SELECT * FROM blockchain_blocks ORDER BY height DESC LIMIT 1");
    if (!$res || $res->num_rows === 0) return null;
    return $res->fetch_assoc();
}

function bcCreateGenesisIfMissing(mysqli $conn): array {
    bcEnsureTable($conn);
    $latest = bcGetLatestBlock($conn);
    if ($latest) return $latest;

    $height = 0;
    $prevHash = str_repeat('0', 64);
    $nonce = 0;
    $createdAt = gmdate('Y-m-d H:i:s');
    $data = [
        'type' => 'genesis',
        'created_by' => 'system',
        'note' => 'Genesis block',
    ];
    $dataJson = json_encode($data, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);
    $hash = bcHashBlockFields($height, $prevHash, $dataJson, $createdAt, $nonce);

    $stmt = $conn->prepare("INSERT INTO blockchain_blocks (height, prev_hash, hash, nonce, data_json, created_at) VALUES (?, ?, ?, ?, ?, ?)");
    $stmt->bind_param("ississ", $height, $prevHash, $hash, $nonce, $dataJson, $createdAt);
    $stmt->execute();

    return bcGetLatestBlock($conn);
}

/**
 * Adds a new block with optional proof-of-work (difficulty = number of leading zeros).
 */
function bcAddBlock(mysqli $conn, array $data, int $difficulty = 0): array {
    $latest = bcCreateGenesisIfMissing($conn);

    $height = intval($latest['height']) + 1;
    $prevHash = strval($latest['hash']);
    $nonce = 0;
    $createdAt = gmdate('Y-m-d H:i:s');
    $dataJson = json_encode($data, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);
    if ($dataJson === false) {
        throw new RuntimeException('Failed to encode data as JSON.');
    }

    $targetPrefix = $difficulty > 0 ? str_repeat('0', $difficulty) : '';
    while (true) {
        $hash = bcHashBlockFields($height, $prevHash, $dataJson, $createdAt, $nonce);
        if ($difficulty <= 0 || strncmp($hash, $targetPrefix, $difficulty) === 0) {
            break;
        }
        $nonce++;
        if ($nonce > 5000000) {
            throw new RuntimeException('PoW limit reached. Lower difficulty.');
        }
    }

    $stmt = $conn->prepare("INSERT INTO blockchain_blocks (height, prev_hash, hash, nonce, data_json, created_at) VALUES (?, ?, ?, ?, ?, ?)");
    $stmt->bind_param("ississ", $height, $prevHash, $hash, $nonce, $dataJson, $createdAt);
    if (!$stmt->execute()) {
        throw new RuntimeException('DB insert failed: ' . $conn->error);
    }

    $row = $conn->query("SELECT * FROM blockchain_blocks WHERE height = " . intval($height))->fetch_assoc();
    return $row;
}

/**
 * Verifies chain integrity from genesis to tip.
 * Returns array with ok(bool) and errors(list).
 */
function bcVerifyChain(mysqli $conn): array {
    bcEnsureTable($conn);

    $errors = [];
    $res = $conn->query("SELECT * FROM blockchain_blocks ORDER BY height ASC");
    if (!$res || $res->num_rows === 0) {
        return ['ok' => false, 'errors' => ['No blocks found.']];
    }

    $prev = null;
    while ($row = $res->fetch_assoc()) {
        $height = intval($row['height']);
        $prevHash = strval($row['prev_hash']);
        $hash = strval($row['hash']);
        $nonce = intval($row['nonce']);
        $dataJson = strval($row['data_json']);
        $createdAt = date('Y-m-d H:i:s', strtotime($row['created_at']));

        $expectedPrevHash = $prev ? strval($prev['hash']) : str_repeat('0', 64);
        if ($prevHash !== $expectedPrevHash) {
            $errors[] = "Block height {$height}: prev_hash mismatch.";
        }

        $expectedHash = bcHashBlockFields($height, $prevHash, $dataJson, $createdAt, $nonce);
        if (!hash_equals($expectedHash, $hash)) {
            $errors[] = "Block height {$height}: hash mismatch (data may be tampered).";
        }

        $prev = $row;
    }

    return ['ok' => count($errors) === 0, 'errors' => $errors];
}

