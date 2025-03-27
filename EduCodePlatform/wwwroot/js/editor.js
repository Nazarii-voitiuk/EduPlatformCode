let cmHtml, cmCss, cmJs;

// Ініціалізація CodeMirror після завантаження DOM
// У Bootstrap 5, ви можете скористатися "DOMContentLoaded", jQuery(document).ready(), тощо.
document.addEventListener("DOMContentLoaded", function () {
    // 1) Ініціалізація HTML-редактора
    cmHtml = CodeMirror.fromTextArea(document.getElementById("htmlEditor"), {
        mode: "htmlmixed",
        lineNumbers: true,
        theme: "neo",
        autoCloseTags: true
    });

    // 2) Ініціалізація CSS-редактора
    cmCss = CodeMirror.fromTextArea(document.getElementById("cssEditor"), {
        mode: "css",
        lineNumbers: true,
        theme: "neo",
        autoCloseTags: true
    });

    // 3) Ініціалізація JS-редактора
    cmJs = CodeMirror.fromTextArea(document.getElementById("jsEditor"), {
        mode: "javascript",
        lineNumbers: true,
        theme: "neo",
        autoCloseTags: true
    });
});

// Кнопка “Run”
function runCode() {
    let html = cmHtml.getValue();
    let css = cmCss.getValue();
    let js = cmJs.getValue();

    let iframe = document.getElementById("previewFrame");
    let iframeDoc = iframe.contentDocument || iframe.contentWindow.document;

    let completeCode = `
        <html>
        <head>
            <style>${css}</style>
        </head>
        <body>
            ${html}
            <script>${js}<\/script>
        </body>
        </html>
    `;

    // Перезаписуємо вміст iframe
    iframeDoc.open();
    iframeDoc.write(completeCode);
    iframeDoc.close();
}

// Кнопка “Save”
function saveCode() {
    let html = cmHtml.getValue();
    let css = cmCss.getValue();
    let js = cmJs.getValue();

    $.ajax({
        url: '/Editor/SaveCode',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            HtmlCode: html,
            CssCode: css,
            JsCode: js
        }),
        success: function (res) {
            alert(res.message + ". SubmissionId = " + res.submissionId);
        },
        error: function (xhr) {
            if (xhr.status == 401) {
                alert("You are not authorized. Please log in.");
            } else {
                alert("Error: " + xhr.responseText);
            }
        }
    });
}
