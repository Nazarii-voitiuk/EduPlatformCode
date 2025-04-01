let cmHtml, cmCss, cmJs;

document.addEventListener("DOMContentLoaded", function () {
    cmHtml = CodeMirror.fromTextArea(document.getElementById("htmlEditor"), {
        mode: "htmlmixed",
        lineNumbers: true,
        theme: "neo",
        autoCloseTags: true
    });
    cmCss = CodeMirror.fromTextArea(document.getElementById("cssEditor"), {
        mode: "css",
        lineNumbers: true,
        theme: "neo",
        autoCloseTags: true
    });
    cmJs = CodeMirror.fromTextArea(document.getElementById("jsEditor"), {
        mode: "javascript",
        lineNumbers: true,
        theme: "neo",
        autoCloseTags: true
    });
});

function runPreview() {
    let htmlVal = cmHtml.getValue();
    let cssVal = cmCss.getValue();
    let jsVal = cmJs.getValue();

    let iframe = document.getElementById("previewFrame");
    let doc = iframe.contentDocument || iframe.contentWindow.document;

    let code = `
      <html>
      <head><style>${cssVal}</style></head>
      <body>
        ${htmlVal}
        <script>${jsVal}<\/script>
      </body>
      </html>
    `;

    doc.open();
    doc.write(code);
    doc.close();
}

function saveSubmission() {
    let submissionId = document.getElementById("submissionId").value;
    let userId = document.getElementById("userSelect").value;
    let titleVal = document.getElementById("titleInput").value;
    let isPublic = document.getElementById("isPublicCheck").checked;

    let htmlVal = cmHtml.getValue();
    let cssVal = cmCss.getValue();
    let jsVal = cmJs.getValue();

    let dataObj = {
        codeSubmissionId: submissionId ? parseInt(submissionId) : null,
        userId: userId,
        title: titleVal,
        isPublic: isPublic,
        htmlCode: htmlVal,
        cssCode: cssVal,
        jsCode: jsVal
    };

    $.ajax({
        url: '/Submissions/Save',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(dataObj),
        success: function (res) {
            alert(res.message + " (ID=" + res.submissionId + ")");
            // redirect to /Submissions/Index or /Submissions/MyProfile
            window.location.href = '/Submissions/Index';
        },
        error: function (xhr) {
            if (xhr.status == 401) {
                alert("Not authorized. Please log in.");
            } else if (xhr.status == 403) {
                alert("Forbidden. Not your submission!");
            } else {
                alert("Error: " + xhr.responseText);
            }
        }
    });
}
