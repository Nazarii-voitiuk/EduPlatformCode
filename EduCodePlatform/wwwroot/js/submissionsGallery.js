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
                  <div class="col-md-4 mb-3">
                    <div class="card shadow-sm">
                      <div class="card-body">
                        <h5 class="card-title">${item.title || 'No Title'}</h5>
                        <p>Owner: ${item.userId}</p>
                        <p>Created: ${formatDate(item.createdAt)}</p>
                        <a href="/Submissions/Editor?submissionId=${item.codeSubmissionId}" class="btn btn-primary btn-sm">
                          View / Fork
                        </a>
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

function formatDate(str) {
    if (!str) return "";
    let d = new Date(str);
    return d.toLocaleString();
}
