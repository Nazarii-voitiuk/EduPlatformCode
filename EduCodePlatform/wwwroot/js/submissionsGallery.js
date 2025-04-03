$(document).ready(function () {
    loadGallery();
});

function loadGallery() {
    $.ajax({
        url: '/Submissions/GetPublic',
        type: 'GET',
        success: function (data) {
            let container = $('#galleryContainer');
            container.empty();

            data.forEach(item => {
                let cardHtml = `
                  <div class="col-md-4 mb-4">
                    <div class="card bg-dark text-light shadow h-100">
                      <div class="card-body d-flex flex-column">
                        <h5 class="card-title">${item.title || 'No Title'}</h5>
                        <p class="mb-1">Owner: ${item.userEmail || ''}</p>
                        <p class="small text-muted">Created: ${formatDate(item.createdAt)}</p>
                        <div class="mt-auto pt-2">
                          <button class="btn btn-outline-primary btn-sm me-2" onclick="previewProject(${item.codeSubmissionId})">
                            Preview
                          </button>
                          <a href="/Submissions/Editor?submissionId=${item.codeSubmissionId}" class="btn btn-primary btn-sm">
                            View / Fork
                          </a>
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

function previewProject(submissionId) {
    // Припустимо, що існує ендпоінт /Submissions/Preview?submissionId=...
    let previewUrl = '/Submissions/Preview?submissionId=' + submissionId;
    $('#previewIframe').attr('src', previewUrl);
    let previewModal = new bootstrap.Modal(document.getElementById('previewModal'));
    previewModal.show();
}

function formatDate(str) {
    if (!str) return "";
    let d = new Date(str);
    return d.toLocaleString();
}
