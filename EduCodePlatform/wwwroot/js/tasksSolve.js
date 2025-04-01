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

    let frame = document.getElementById("previewFrame");
    let doc = frame.contentDocument || frame.contentWindow.document;

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

function checkSolution() {
    let taskId = parseInt(document.getElementById("taskId").value);
    let dataObj = {
        taskId: taskId,
        htmlCode: cmHtml.getValue(),
        cssCode: cmCss.getValue(),
        jsCode: cmJs.getValue()
    };

    $.ajax({
        url: '/Tasks/CheckSolution',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(dataObj),
        success: function (res) {
            if (res.passed) {
                alert("All testcases passed! Score=" + res.score);
            } else {
                alert("Some testcases failed. Score=" + res.score);
            }
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}
