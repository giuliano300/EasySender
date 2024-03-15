﻿function validateForm() {
    return CompareDate($('.startDate').val());
};

function CompareDate(startDate) {
    var d = new Date(startDate);
    var now = new Date();
    if (d <= now) {
        alert("Inserire una data successiva ad oggi.");
        return false;
    }
    return true;
}

function DeleteOrReplaceBadWords(str) {
    var xbadChars = new Array('\u00AB', '\u00BB', '\u201C', '\u201D', '\u02BA', '\u02EE', '\u201F', '\u275D', '\u275E', '\u301D', '\u301E', '\uFF02', '\u2018', '\u2019', '\u02BB', '\u02C8', '\u02BC', '\u02BD', '\u02B9', '\u201B', '\uFF07', '\u00B4', '\u02CA', '\u0060', '\u02CB', '\u275B', '\u275C', '\u0313', '\u0314', '\uFE10', '\uFE11', '\u00F7', '\u00BC', '\u00BD', '\u00BE', '\u29F8', '\u0337', '\u0338', '\u2044', '\u2215', '\uFF0F', '\u0332', '\uFF3F', '\u20D2', '\u20D3', '\u2223', '\uFF5C', '\u23B8', '\u23B9', '\u23D0', '\u239C', '\u239F', '\u23BC', '\u23BD', '\u2015', '\uFE63', '\uFF0D', '\u2010', '\u2043', '\uFE6B', '\uFF20', '\uFE69', '\uFF04', '\u01C3', '\uFE15', '\uFE57', '\uFF01', '\uFE5F', '\uFF03', '\uFE6A', '\uFF05', '\uFE60', '\uFF06', '\u201A', '\u0326', '\uFE50', '\uFE51', '\uFF0C', '\uFF64', '\u2768', '\u276A', '\uFE59', '\uFF08', '\u27EE', '\u2985', '\u2769', '\u276B', '\uFE5A', '\uFF09', '\u27EF', '\u2986', '\u204E', '\u2217', '\u229B', '\u2722', '\u2723', '\u2724', '\u2725', '\u2731', '\u2732', '\u2733', '\u273A', '\u273B', '\u273C', '\u273D', '\u2743', '\u2749', '\u274A', '\u274B', '\u29C6', '\uFE61', '\uFF0A', '\u02D6', '\uFE62', '\uFF0B', '\u3002', '\uFE52', '\uFF0E', '\uFF61', '\uFF10', '\uFF11', '\uFF12', '\uFF13', '\uFF14', '\uFF15', '\uFF16', '\uFF17', '\uFF18', '\uFF19', '\u02D0', '\u02F8', '\u2982', '\uA789', '\uFE13', '\uFF1A', '\u204F', '\uFE14', '\uFE54', '\uFF1B', '\uFE64', '\uFF1C', '\u0347', '\uA78A', '\uFE66', '\uFF1D', '\uFE65', '\uFF1E', '\uFE16', '\uFE56', '\uFF1F', '\uFF21', '\u1D00', '\uFF22', '\u0299', '\uFF23', '\u1D04', '\uFF24', '\u1D05', '\uFF25', '\u1D07', '\uFF26', '\uA730', '\uFF27', '\u0262', '\uFF28', '\u029C', '\uFF29', '\u026A', '\uFF2A', '\u1D0A', '\uFF2B', '\u1D0B', '\uFF2C', '\u029F', '\uFF2D', '\u1D0D', '\uFF2E', '\u0274', '\uFF2F', '\u1D0F', '\uFF30', '\u1D18', '\uFF31', '\uFF32', '\u0280', '\uFF33', '\uA731', '\uFF34', '\u1D1B', '\uFF35', '\u1D1C', '\uFF36', '\u1D20', '\uFF37', '\u1D21', '\uFF38', '\uFF39', '\u028F', '\uFF3A', '\u1D22', '\u02C6', '\u0302', '\uFF3E', '\u1DCD', '\u2774', '\uFE5B', '\uFF5B', '\u2775', '\uFE5C', '\uFF5D', '\uFF3B', '\uFF3D', '\u02DC', '\u02F7', '\u0303', '\u0330', '\u0334', '\u223C', '\uFF5E', '\u00A0', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A', '\u202F', '\u205F', '\u3000', '\u008D', '\u009F', '\u0080', '\u0090', '\u009B', '\u0010', '\u0009', '\u0000', '\u0003', '\u0004', '\u0017', '\u0019', '\u0011', '\u0012', '\u0013', '\u0014', '\u2017', '\u2014', '\u2013', '\u201A', '\u202F', '\u2039', '\u203A', '\u203C', '\u201E', '\u201D', '\u201C', '\u201B', '\u2026', '\u2028', '\u2029', '\u205F', '\u2060');

    var xgoodChars = new Array('"', '"', '"', '"', '"', '"', '"', '"', '"', '"', '"', '"', "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", "'", '/', '1/4', '1/2', '3/4', '/', '/', '/', '/', '/', '/', '_', '_', '|', '|', '|', '|', '|', '|', '|', '|', '|', '-', '-', '-', '-', '-', '-', '-', '@', '@', '$', '$', '!', '!', '!', '!', '#', '#', '%', '%', '&', '&', ',', ',', ',', ',', ',', ',', '(', '(', '(', '(', '(', '(', ')', ')', ')', ')', ')', ')', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '*', '+', '+', '+', '.', '.', '.', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ':', ':', ':', ':', ':', ';', ';', ';', ';', '<', '<', '=', '=', '=', '=', '>', '>', '?', '?', '?', 'A', 'A', 'B', 'B', 'C', 'C', 'D', 'D', 'E', 'E', 'F', 'F', 'G', 'G', 'H', 'H', 'I', 'I', 'J', 'J', 'K', 'K', 'L', 'L', 'M', 'M', 'N', 'N', 'O', 'O', 'P', 'P', 'Q', 'R', 'R', 'S', 'S', 'T', 'T', 'U', 'U', 'V', 'V', 'W', 'W', 'X', 'Y', 'Y', 'Z', 'Z', '^', '^', '^', '^', '{', '{', '{', '}', '}', '}', '[', ']', '~', '~', '~', '~', '~', '~', '~', "'", "'", ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '-', '-', "'", '', '<', '>', '!!', '"', '"', '"', "'", '...', ' ', ' ', ' ', ' ');

    xbadChars.forEach(function (element, index) {
        if (str.includes(element)) {
            str = str.replace(element, xgoodChars[index]);
        }
    });
    $('.message').val(str);
}

function CreateTblBadWords() {
    var c = 0;
    var tbl = $('.tbl');
    var xbadChars = new Array('\u00AB', '\u00BB', '\u201C', '\u201D', '\u02BA', '\u02EE', '\u201F', '\u275D', '\u275E', '\u301D', '\u301E', '\uFF02', '\u2018', '\u2019', '\u02BB', '\u02C8', '\u02BC', '\u02BD', '\u02B9', '\u201B', '\uFF07', '\u00B4', '\u02CA', '\u0060', '\u02CB', '\u275B', '\u275C', '\u0313', '\u0314', '\uFE10', '\uFE11', '\u00F7', '\u00BC', '\u00BD', '\u00BE', '\u29F8', '\u0337', '\u0338', '\u2044', '\u2215', '\uFF0F', '\u0332', '\uFF3F', '\u20D2', '\u20D3', '\u2223', '\uFF5C', '\u23B8', '\u23B9', '\u23D0', '\u239C', '\u239F', '\u23BC', '\u23BD', '\u2015', '\uFE63', '\uFF0D', '\u2010', '\u2043', '\uFE6B', '\uFF20', '\uFE69', '\uFF04', '\u01C3', '\uFE15', '\uFE57', '\uFF01', '\uFE5F', '\uFF03', '\uFE6A', '\uFF05', '\uFE60', '\uFF06', '\u201A', '\u0326', '\uFE50', '\uFE51', '\uFF0C', '\uFF64', '\u2768', '\u276A', '\uFE59', '\uFF08', '\u27EE', '\u2985', '\u2769', '\u276B', '\uFE5A', '\uFF09', '\u27EF', '\u2986', '\u204E', '\u2217', '\u229B', '\u2722', '\u2723', '\u2724', '\u2725', '\u2731', '\u2732', '\u2733', '\u273A', '\u273B', '\u273C', '\u273D', '\u2743', '\u2749', '\u274A', '\u274B', '\u29C6', '\uFE61', '\uFF0A', '\u02D6', '\uFE62', '\uFF0B', '\u3002', '\uFE52', '\uFF0E', '\uFF61', '\uFF10', '\uFF11', '\uFF12', '\uFF13', '\uFF14', '\uFF15', '\uFF16', '\uFF17', '\uFF18', '\uFF19', '\u02D0', '\u02F8', '\u2982', '\uA789', '\uFE13', '\uFF1A', '\u204F', '\uFE14', '\uFE54', '\uFF1B', '\uFE64', '\uFF1C', '\u0347', '\uA78A', '\uFE66', '\uFF1D', '\uFE65', '\uFF1E', '\uFE16', '\uFE56', '\uFF1F', '\uFF21', '\u1D00', '\uFF22', '\u0299', '\uFF23', '\u1D04', '\uFF24', '\u1D05', '\uFF25', '\u1D07', '\uFF26', '\uA730', '\uFF27', '\u0262', '\uFF28', '\u029C', '\uFF29', '\u026A', '\uFF2A', '\u1D0A', '\uFF2B', '\u1D0B', '\uFF2C', '\u029F', '\uFF2D', '\u1D0D', '\uFF2E', '\u0274', '\uFF2F', '\u1D0F', '\uFF30', '\u1D18', '\uFF31', '\uFF32', '\u0280', '\uFF33', '\uA731', '\uFF34', '\u1D1B', '\uFF35', '\u1D1C', '\uFF36', '\u1D20', '\uFF37', '\u1D21', '\uFF38', '\uFF39', '\u028F', '\uFF3A', '\u1D22', '\u02C6', '\u0302', '\uFF3E', '\u1DCD', '\u2774', '\uFE5B', '\uFF5B', '\u2775', '\uFE5C', '\uFF5D', '\uFF3B', '\uFF3D', '\u02DC', '\u02F7', '\u0303', '\u0330', '\u0334', '\u223C', '\uFF5E', '\u00A0', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A', '\u202F', '\u205F', '\u3000', '\u008D', '\u009F', '\u0080', '\u0090', '\u009B', '\u0010', '\u0009', '\u0000', '\u0003', '\u0004', '\u0017', '\u0019', '\u0011', '\u0012', '\u0013', '\u0014', '\u2017', '\u2014', '\u2013', '\u201A', '\u202F', '\u2039', '\u203A', '\u203C', '\u201E', '\u201D', '\u201C', '\u201B', '\u2026', '\u2028', '\u2029', '\u205F', '\u2060');

    xbadChars.forEach(function (element) {
        $(document.createElement('span')).html(element).appendTo(tbl);
    });

}

function openTbl() {
    $('.container-bad-words').show();
}
function closeTbl() {
    $('.container-bad-words').hide();
}