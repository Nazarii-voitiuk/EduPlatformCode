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
                let cardHtml = `
                  <div class="col-md-4 mb-4">
                    <div class="card bg-dark text-light shadow h-100">
                      <div class="card-body d-flex flex-column">
                        <h5 class="card-title">${item.title || 'No Title'}</h5>
                        <p class="mb-1">Public: ${item.isPublic}</p>
                        <p class="small text-muted">Owner: ${item.userEmail || ''}</p>
                        <p class="small text-muted">UpdatedAt: ${formatDate(item.updatedAt)}</p>
                        <div class="mt-auto pt-2">
                          <button class="btn btn-info btn-sm me-2" onclick="goEdit(${item.codeSubmissionId})">Edit</button>
                          <button class="btn btn-danger btn-sm" onclick="deleteSubmission(${item.codeSubmissionId})">Delete</button>
                        </div>
                      </div>
                    </div>
                  </div>
                `;
                container.append(cardHtml);
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


// Використовуємо ту ж функцію попереднього перегляду
function previewProject(submissionId) {
    let previewUrl = '/Submissions/Preview?submissionId=' + submissionId;
    $('#previewIframe').attr('src', previewUrl);
    let previewModal = new bootstrap.Modal(document.getElementById('previewModal'));
    previewModal.show();
}
