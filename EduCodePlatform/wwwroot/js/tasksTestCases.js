let tcEditMode = false;

$(document).ready(function () {
    loadTestCases();
});

function loadTestCases() {
    let taskId = $('#TaskId').val();
    $.ajax({
        url: '/Tasks/GetTestCases?taskId=' + taskId,
        type: 'GET',
        success: function (data) {
            let tbody = $('#testCasesTable tbody');
            tbody.empty();
            data.forEach(tc => {
                let row = `
                  <tr>
                    <td>${tc.testCaseId}</td>
                    <td>${tc.htmlRules || ''}</td>
                    <td>${tc.cssRules || ''}</td>
                    <td>${tc.inputData || ''}</td>
                    <td>${tc.expectedJsOutput || ''}</td>
                    <td>${tc.timeLimitSeconds}</td>
                    <td>${tc.points}</td>
                    <td>
                      <button class="btn btn-sm btn-info" onclick="openEditTcModal(${tc.testCaseId}, '${escapeStr(tc.htmlRules)}', '${escapeStr(tc.cssRules)}', '${escapeStr(tc.inputData)}','${escapeStr(tc.expectedJsOutput)}', ${tc.timeLimitSeconds}, ${tc.points})">Edit</button>
                      <button class="btn btn-sm btn-danger" onclick="deleteTestCase(${tc.testCaseId})">Delete</button>
                    </td>
                  </tr>`;
                tbody.append(row);
            });
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

function openCreateTcModal() {
    tcEditMode = false;
    clearTcModal();
    $('#tcModalLabel').text("Create TestCase");
    $('#tcModal').modal('show');
}

function openEditTcModal(id, htmlR, cssR, inp, exp, timeSec, pts) {
    tcEditMode = true;
    clearTcModal();
    $('#TestCaseId').val(id);
    $('#HtmlRules').val(htmlR);
    $('#CssRules').val(cssR);
    $('#InputData').val(inp);
    $('#ExpectedJsOutput').val(exp);
    $('#TimeLimitSeconds').val(timeSec);
    $('#Points').val(pts);
    $('#tcModalLabel').text("Edit TestCase #" + id);
    $('#tcModal').modal('show');
}

function clearTcModal() {
    $('#TestCaseId').val('');
    $('#HtmlRules').val('');
    $('#CssRules').val('');
    $('#InputData').val('');
    $('#ExpectedJsOutput').val('');
    $('#TimeLimitSeconds').val('2');
    $('#Points').val('10');
}

function saveTestCase() {
    let tid = parseInt($('#TaskId').val());
    let tcid = $('#TestCaseId').val();
    let dataObj = {
        testCaseId: tcid ? parseInt(tcid) : 0,
        taskId: tid,
        htmlRules: $('#HtmlRules').val(),
        cssRules: $('#CssRules').val(),
        inputData: $('#InputData').val(),
        expectedJsOutput: $('#ExpectedJsOutput').val(),
        timeLimitSeconds: parseInt($('#TimeLimitSeconds').val()),
        points: parseInt($('#Points').val())
    };

    if (!tcEditMode) {
        // create
        $.ajax({
            url: '/Tasks/CreateTestCaseAjax',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                $('#tcModal').modal('hide');
                loadTestCases();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        // edit
        $.ajax({
            url: '/Tasks/EditTestCaseAjax',
            type: 'PUT',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                $('#tcModal').modal('hide');
                loadTestCases();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    }
}

function deleteTestCase(id) {
    if (!confirm("Delete testCase #" + id + "?")) return;
    $.ajax({
        url: '/Tasks/DeleteTestCaseAjax/' + id,
        type: 'DELETE',
        success: function (res) {
            alert(res.message);
            loadTestCases();
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

function escapeStr(str) {
    if (!str) return "";
    return str.replace(/`/g, "\\`").replace(/\$/g, "\\$");
}
