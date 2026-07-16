<?php
require_once 'includes/config.php';
require_once 'includes/auth.php';
requireLogin();

require_once 'blockchain_lib.php';

$message = '';
$message_type = 'info';

try {
    bcEnsureTable($conn);
    bcCreateGenesisIfMissing($conn);
    $message = 'Blockchain table is ready and genesis block exists.';
    $message_type = 'success';
} catch (Exception $e) {
    $message = 'Setup failed: ' . $e->getMessage();
    $message_type = 'danger';
}

$page_title = 'Blockchain Setup';
include 'includes/header.php';
?>

<h2><i class="fas fa-link"></i> Blockchain Setup</h2>

<div class="alert alert-<?php echo $message_type; ?>">
    <?php echo htmlspecialchars($message); ?>
</div>

<a class="btn btn-primary" href="blockchain_view.php">Go to Blockchain</a>
<a class="btn btn-secondary" href="dashboard.php">Back to Dashboard</a>

<?php include 'includes/footer.php'; ?>

