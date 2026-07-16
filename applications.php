<?php
require_once 'includes/config.php';
require_once 'includes/smtp_send.php';
require_once 'includes/auth.php';
requireLogin();
include 'includes/header.php';

// Handle form submissions (status updates)
if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_POST['action'])) {
    switch ($_POST['action']) {
        case 'update_status':
            $id = intval($_POST['id'] ?? 0);
            $new_status = $_POST['new_status'] ?? '';

            $allowed = ['pending', 'reviewed', 'shortlisted', 'interviewed', 'hired', 'rejected'];
            if (!in_array($new_status, $allowed, true) || $id <= 0) {
                $message = 'Invalid application update request.';
                $message_type = 'danger';
                break;
            }

            $old_status = '';
            $q = $conn->prepare('SELECT status FROM applications WHERE id = ? LIMIT 1');
            if ($q) {
                $q->bind_param('i', $id);
                $q->execute();
                $r = $q->get_result();
                if ($r && ($row = $r->fetch_assoc())) {
                    $old_status = (string)($row['status'] ?? '');
                }
                $q->close();
            }

            $stmt = $conn->prepare("UPDATE applications SET status = ? WHERE id = ?");
            $stmt->bind_param("si", $new_status, $id);

            if ($stmt->execute()) {
                $message = 'Application status updated successfully!';
                $message_type = 'success';
                careerconnect_notify_application_status($conn, $id, $old_status, $new_status);
            } else {
                $message = 'Error updating application status: ' . $conn->error;
                $message_type = 'danger';
            }
            break;
    }
}

// Pagination
$limit = 10;
$page = isset($_GET['page']) ? max(1, intval($_GET['page'])) : 1;
$offset = ($page - 1) * $limit;

$total_result = $conn->query("SELECT COUNT(*) as total FROM applications");
$total_applications = $total_result->fetch_assoc()['total'];
$total_pages = max(1, ceil($total_applications / $limit));

// Fetch applications with user, job, and company details
$result = $conn->query("
    SELECT 
        a.id, 
        a.cover_letter, 
        a.status, 
        a.applied_at,
        CONCAT(u.first_name, ' ', u.last_name) as user_name,
        u.email as user_email,
        j.title as job_title,
        c.name as company_name
    FROM applications a
    LEFT JOIN users u ON a.user_id = u.id
    LEFT JOIN jobs j ON a.job_id = j.id
    LEFT JOIN companies c ON j.company_id = c.id
    ORDER BY a.applied_at DESC
    LIMIT $limit OFFSET $offset
");
?>

<div class="d-flex justify-content-between align-items-center mb-4">
    <h2><i class="fas fa-file-alt"></i> Job Applications</h2>
    <div class="d-flex gap-2">
        <span class="badge bg-primary">Total: <?php echo $total_applications; ?></span>
    </div>
</div>

<?php if (isset($message)): ?>
    <div class="alert alert-<?php echo $message_type; ?> alert-dismissible fade show" role="alert">
        <?php echo htmlspecialchars($message); ?>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
<?php endif; ?>

<div class="card">
    <div class="card-header">
        <h5 class="mb-0">All Applications</h5>
    </div>
    <div class="card-body">
        <div class="table-responsive">
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Applicant</th>
                        <th>Job Title</th>
                        <th>Company</th>
                        <th>Cover Letter</th>
                        <th>Status</th>
                        <th>Applied Date</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <?php while ($application = $result->fetch_assoc()): ?>
                        <tr>
                            <td><?php echo $application['id']; ?></td>
                            <td>
                                <div>
                                    <strong><?php echo htmlspecialchars($application['user_name'] ?: 'Unknown'); ?></strong><br>
                                    <small class="text-muted"><?php echo htmlspecialchars($application['user_email'] ?: ''); ?></small>
                                </div>
                            </td>
                            <td><?php echo htmlspecialchars($application['job_title'] ?: ''); ?></td>
                            <td><?php echo htmlspecialchars($application['company_name'] ?: ''); ?></td>
                            <td>
                                <?php if (!empty($application['cover_letter'])): ?>
                                    <span title="<?php echo htmlspecialchars($application['cover_letter']); ?>">
                                        <?php echo htmlspecialchars(substr($application['cover_letter'], 0, 60)) . (strlen($application['cover_letter']) > 60 ? '...' : ''); ?>
                                    </span>
                                <?php else: ?>
                                    <span class="text-muted">No cover letter</span>
                                <?php endif; ?>
                            </td>
                            <td>
                                <span class="badge bg-<?php
                                    $status = strtolower($application['status'] ?? '');
                                    $badgeClass = 'secondary';
                                    switch ($status) {
                                        case 'pending':
                                            $badgeClass = 'warning';
                                            break;
                                        case 'reviewed':
                                            $badgeClass = 'info';
                                            break;
                                        case 'shortlisted':
                                            $badgeClass = 'primary';
                                            break;
                                        case 'interviewed':
                                            $badgeClass = 'secondary';
                                            break;
                                        case 'hired':
                                            $badgeClass = 'success';
                                            break;
                                        case 'rejected':
                                            $badgeClass = 'danger';
                                            break;
                                        default:
                                            $badgeClass = 'secondary';
                                    }
                                    echo $badgeClass;
                                ?>">
                                    <?php echo ucfirst(htmlspecialchars($application['status'] ?: 'pending')); ?>
                                </span>
                            </td>
                            <td><?php echo date('M d, Y H:i', strtotime($application['applied_at'])); ?></td>
                            <td>
                                <div class="btn-group" role="group">
                                    <?php
                                        $current = strtolower($application['status'] ?? '');
                                        $canAccept = in_array($current, ['pending', 'reviewed'], true);
                                    ?>
                                    <form method="POST" style="display: inline;">
                                        <input type="hidden" name="action" value="update_status">
                                        <input type="hidden" name="id" value="<?php echo $application['id']; ?>">
                                        <input type="hidden" name="new_status" value="reviewed">
                                        <button
                                            type="submit"
                                            class="btn btn-sm btn-success"
                                            title="Accept"
                                            <?php echo $canAccept ? '' : 'disabled'; ?>
                                        >
                                            <i class="fas fa-check"></i>
                                        </button>
                                    </form>
                                    <form method="POST" style="display: inline;">
                                        <input type="hidden" name="action" value="update_status">
                                        <input type="hidden" name="id" value="<?php echo $application['id']; ?>">
                                        <input type="hidden" name="new_status" value="rejected">
                                        <button type="submit" class="btn btn-sm btn-danger" title="Reject">
                                            <i class="fas fa-times"></i>
                                        </button>
                                    </form>
                                </div>
                            </td>
                        </tr>
                    <?php endwhile; ?>
                </tbody>
            </table>
        </div>

        <?php if ($total_pages > 1): ?>
            <div class="d-flex justify-content-center mt-3">
                <nav>
                    <ul class="pagination">
                        <?php for ($i = 1; $i <= $total_pages; $i++): ?>
                            <li class="page-item<?php echo $i == $page ? ' active' : ''; ?>">
                                <a class="page-link" href="applications.php?page=<?php echo $i; ?>"><?php echo $i; ?></a>
                            </li>
                        <?php endfor; ?>
                    </ul>
                </nav>
            </div>
        <?php endif; ?>
    </div>
</div>

<?php include 'includes/footer.php'; ?>