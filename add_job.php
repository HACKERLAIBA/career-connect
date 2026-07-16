
<?php
require_once 'includes/config.php';
require_once 'includes/auth.php';
requireLogin();
include 'includes/header.php';

// Fetch companies and categories for dropdowns
$companies = $conn->query("SELECT id, name FROM companies ORDER BY name");
$categories = $conn->query("SELECT id, name FROM categories ORDER BY name");

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $title = trim($_POST['title'] ?? '');
    $company_id = intval($_POST['company_id'] ?? 0);
    $category_id = intval($_POST['category_id'] ?? 0);
    $location = trim($_POST['location'] ?? '');
    $job_type = trim($_POST['job_type'] ?? '');
    $status = trim($_POST['status'] ?? '');

    $description = trim($_POST['description'] ?? '');
    $requirements = trim($_POST['requirements'] ?? '');
    $salary_min = trim($_POST['salary_min'] ?? '');
    $salary_max = trim($_POST['salary_max'] ?? '');
    $work_arrangement = trim($_POST['work_arrangement'] ?? 'onsite');
    if (!in_array($work_arrangement, ['onsite', 'remote', 'hybrid'], true)) {
        $work_arrangement = 'onsite';
    }
    $how_to_apply = trim($_POST['how_to_apply'] ?? '');
    $application_deadline = trim($_POST['application_deadline'] ?? '');

    // Allow empty salary fields => store NULL
    $salary_min_val = $salary_min === '' ? null : $salary_min;
    $salary_max_val = $salary_max === '' ? null : $salary_max;
    $deadline_val = $application_deadline === '' ? null : $application_deadline;

    // jobs.description is NOT NULL in schema, so ensure it's at least an empty string
    if ($description === '') {
        $description = '';
    }

    $stmt = $conn->prepare("
        INSERT INTO jobs (
            title, company_id, category_id, location, job_type, status,
            description, requirements, salary_min, salary_max, created_by,
            work_arrangement, how_to_apply, application_deadline
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 1, ?, ?, ?)
    ");
    $stmt->bind_param(
        "siissssssssss",
        $title,
        $company_id,
        $category_id,
        $location,
        $job_type,
        $status,
        $description,
        $requirements,
        $salary_min_val,
        $salary_max_val,
        $work_arrangement,
        $how_to_apply,
        $deadline_val
    );

    if ($stmt->execute()) {
        echo '<div class="alert alert-success">Job added successfully.</div>';
    } else {
        echo '<div class="alert alert-danger">Error adding job: ' . htmlspecialchars($conn->error) . '</div>';
    }
}
?>
<h2>Add Job</h2>
<form method="POST">
    <div class="mb-3">
        <label>Title</label>
        <input type="text" name="title" class="form-control" required>
    </div>
    <div class="mb-3">
        <label>Company</label>
        <select name="company_id" class="form-control" required>
            <?php while ($c = $companies->fetch_assoc()): ?>
                <option value="<?= $c['id'] ?>"><?= htmlspecialchars($c['name']) ?></option>
            <?php endwhile; ?>
        </select>
    </div>
    <div class="mb-3">
        <label>Category</label>
        <select name="category_id" class="form-control" required>
            <?php while ($cat = $categories->fetch_assoc()): ?>
                <option value="<?= $cat['id'] ?>"><?= htmlspecialchars($cat['name']) ?></option>
            <?php endwhile; ?>
        </select>
    </div>
    <div class="mb-3">
        <label>Location</label>
        <input type="text" name="location" class="form-control">
    </div>
    <div class="mb-3">
        <label>Job Type</label>
        <select name="job_type" class="form-control">
            <option value="full-time">Full-time</option>
            <option value="part-time">Part-time</option>
            <option value="contract">Contract</option>
            <option value="internship">Internship</option>
            <option value="freelance">Freelance</option>
        </select>
    </div>
    <div class="mb-3">
        <label>Status</label>
        <select name="status" class="form-control">
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
            <option value="closed">Closed</option>
        </select>
    </div>

    <div class="row">
        <div class="col-md-6">
            <div class="mb-3">
                <label>Salary Min (optional)</label>
                <input type="number" name="salary_min" class="form-control" step="0.01" min="0">
            </div>
        </div>
        <div class="col-md-6">
            <div class="mb-3">
                <label>Salary Max (optional)</label>
                <input type="number" name="salary_max" class="form-control" step="0.01" min="0">
            </div>
        </div>
    </div>

    <div class="mb-3">
        <label>Description</label>
        <textarea name="description" class="form-control" rows="4" placeholder="Job description"></textarea>
    </div>

    <div class="mb-3">
        <label>Requirements</label>
        <textarea name="requirements" class="form-control" rows="4" placeholder="Job requirements"></textarea>
    </div>

    <div class="mb-3">
        <label>Work arrangement</label>
        <select name="work_arrangement" class="form-control">
            <option value="onsite">On-site</option>
            <option value="remote">Remote</option>
            <option value="hybrid">Hybrid</option>
        </select>
    </div>
    <div class="mb-3">
        <label>Application deadline (optional)</label>
        <input type="date" name="application_deadline" class="form-control">
    </div>
    <div class="mb-3">
        <label>How to apply (optional)</label>
        <textarea name="how_to_apply" class="form-control" rows="3" placeholder="Instructions for candidates"></textarea>
    </div>

    <button type="submit" class="btn btn-success">Add Job</button>
    <a href="jobs.php" class="btn btn-secondary">Back</a>
</form>
<?php include 'includes/footer.php'; ?>