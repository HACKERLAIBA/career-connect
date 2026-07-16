<?php
$page_title = 'Approval Setup';

require_once 'includes/auth.php';
requireLogin();
require_once 'includes/header.php';
require_once 'includes/config.php';

$logs = [];
$ok = true;

function runSql($conn, $sql, &$logs, &$ok) {
    if ($conn->query($sql) === true) {
        $logs[] = ['type' => 'success', 'text' => $sql];
        return true;
    }
    $ok = false;
    $logs[] = ['type' => 'danger', 'text' => $sql . " — ERROR: " . $conn->error];
    return false;
}

// Add status columns if missing (MySQL 5.7 compatible)
runSql($conn, "ALTER TABLE companies ADD COLUMN status VARCHAR(20) NULL", $logs, $ok);
runSql($conn, "ALTER TABLE jobs ADD COLUMN status VARCHAR(20) NULL", $logs, $ok);

// Ensure NULL/empty statuses become pending (so Approve buttons show)
runSql($conn, "UPDATE companies SET status = 'pending' WHERE status IS NULL OR status = ''", $logs, $ok);
runSql($conn, "UPDATE jobs SET status = 'pending' WHERE status IS NULL OR status = ''", $logs, $ok);

?>

<div class="container-fluid p-4">
    <h2 class="mb-2">Setup — Company/Job approvals</h2>
    <p class="text-muted mb-4">Run once to ensure required columns exist and pending statuses are set.</p>

    <div class="alert alert-<?php echo $ok ? 'success' : 'warning'; ?>">
        <?php echo $ok ? 'Setup finished (some ALTER statements may show errors if already applied).' : 'Setup finished with some errors. See details below.'; ?>
    </div>

    <div class="card">
        <div class="card-header"><strong>Executed SQL</strong></div>
        <div class="card-body">
            <ul class="mb-0">
                <?php foreach ($logs as $l): ?>
                    <li class="text-<?php echo htmlspecialchars($l['type']); ?>" style="font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, 'Liberation Mono', 'Courier New', monospace;">
                        <?php echo htmlspecialchars($l['text']); ?>
                    </li>
                <?php endforeach; ?>
            </ul>
        </div>
    </div>

    <div class="mt-3 d-flex gap-2 flex-wrap">
        <a class="btn btn-primary" href="companies.php">Go to Companies</a>
        <a class="btn btn-outline-primary" href="jobs.php">Go to Jobs</a>
    </div>
</div>

<?php require_once 'includes/footer.php'; ?>

