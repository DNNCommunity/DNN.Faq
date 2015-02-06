function GetFaqAnswerSuccess(result, ctx) {
    var lblAnswer = dnn.dom.getById(ctx);
    lblAnswer.innerHTML = result;
}

function GetFaqAnswerError(result, ctx) {
    var lblAnswer = dnn.dom.getById(ctx);
    lblAnswer.innerHTML = result;

}

//Show/hide answer label
function SetAnswerLabel(answerClientId) {
    try {
        var label = document.getElementById(answerClientId);
        if (label != null) {
            if (label.innerHTML == '') {

                var ClientCallBackRef = eval('ClientCallBackRef' + answerClientId);
                var LoadingTemplate = eval('LoadingTemplate' + answerClientId);

                label.innerHTML = LoadingTemplate;
                eval(ClientCallBackRef);
            } else {
                label.innerHTML = '';
            }
        }
    } catch (e) {
        alert("Error in function SetAnswerLabel:" + e.stack);
    }
}
