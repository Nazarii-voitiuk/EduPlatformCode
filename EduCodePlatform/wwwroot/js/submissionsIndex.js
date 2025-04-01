$(document).ready(function () {
    loadSubmissions();
});

function loadSubmissions() {
    $.ajax({
        url: '/Submissions/GetAll',
        type: 'GET',
        success: function (data) {
            let tbody = $('#submissionsTable tbody');
            tbody.empty();

            data.forEach(item => {
                let row = `
                  <tr>
                    <td>${item.codeSubmissionId}</td>
                    <td>${item.userId}</td>
                    <td>${item.title || ''}</td>
                    <td>${item.isPublic}</td>
                    <td>${formatDate(item.createdAt)}</td>
                    <td>
                      <button class="btn btn-sm btn-info" onclick="goEdit(${item.codeSubmissionId})">Edit</button>
                      <button class="btn btn-sm btn-danger" onclick="deleteSubmission(${item.codeSubmissionId})">Delete</button>
                    </td>
                  </tr>
                `;
                tbody.append(row);
            });
        },
        error: function (xhr) {
            alert("Error loading: " + xhr.responseText);
        }
    });
}

function goCreate() {
    window.location.href = '/Submissions/Editor';
}

function goEdit(id) {
    window.location.href = '/Submissions/Editor?submissionId=' + id;
}

function deleteSubmission(id) {
    if (!confirm("Delete submission #" + id + "?")) return;
    $.ajax({
        url: '/Submissions/Delete/' + id,
        type: 'DELETE',
        success: function (res) {
            alert(res.message);
            loadSubmissions();
        },
        error: function (xhr) {
            if (xhr.status == 403) {
                alert("Forbidden. Not yours!");
            } else {
                alert("Error: " + xhr.responseText);
            }
        }
    });
}

function formatDate(str) {
    if (!str) return "";
    let d = new Date(str);
    return d.toLocaleString();
}
