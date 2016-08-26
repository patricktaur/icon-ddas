Function()
{
    var doc = new jsPDF();

    var specialElementHandlers = {
        '#editor': function (element, renderer) {
            return true;
        }
    };

    doc.fromHTML($('#render_me').get(0), 15, 15, {
        'width': 170,
        'elementHandlers': specialElementHandlers
    });

    $('a').click(function () {
        alert("downloading...");
        doc.save('TestHTMLDoc.pdf');
    });
}