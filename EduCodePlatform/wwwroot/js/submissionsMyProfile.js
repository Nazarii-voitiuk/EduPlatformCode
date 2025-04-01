$(document).ready(function () {
    loadMySubmissions();
});

function loadMySubmissions() {
    $.ajax({
        url: '/Submissions/GetMy',
        type: 'GET',
        success: function (data) {
            let container = $('#mySubmissionsContainer');
            container.empty();

            data.forEach(item => {
                let card = `
                  <div class="col-md-4 mb-3">
                    <div class="card shadow-sm">
                      <div class="card-body">
                        <h5>${item.title || 'No Title'}</h5>
                        <p>Public: ${item.isPublic}</p>
                        <p>UpdatedAt: ${formatDate(item.updatedAt)}</p>
                        <button class="btn btn-info btn-sm" onclick="goEdit(${item.codeSubmissionId})">Edit</button>
                        <button class="btn btn-danger btn-sm" onclick="deleteSubmission(${item.codeSubmissionId})">Delete</button>
                      </div>
                    </div>
                  </div>
                `;
                container.append(card);
            });
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
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
            loadMySubmissions();
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
