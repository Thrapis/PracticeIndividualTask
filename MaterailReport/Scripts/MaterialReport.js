function hasMaterial(value) {
    let xhr = new XMLHttpRequest();
    xhr.open("GET", "/Home/HasMaterial?objectName=" + (value), true);
    xhr.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status === 200) {
                //alert(this.responseText);
                if (this.responseText === 'False')
                    document.getElementById('no_material').style['opacity'] = 1;
                else
                    document.getElementById('no_material').style['opacity'] = 0;
            } else alert("status = " +
                this.status + "\n" + this.statusText);
        }
    }
    xhr.send(null);
}


function checkBox(number, list, diff) {

    var chboxname = (diff + 'chb ' + (number));
    var row = (diff + 'row ' + (number));
    var id = (list + (number));

    if (document.getElementById(chboxname).checked) {

        var dinp = document.getElementById(id);
        dinp.parentNode.removeChild(dinp);

        document.getElementById(row).style["background-color"] = '';
    }
        
    else {
        var inp = document.createElement('input');
        inp.setAttribute("type", "text");
        inp.setAttribute("name", "nums");
        inp.setAttribute("id", id);
        inp.setAttribute("value", (number));
        document.getElementById(list).appendChild(inp);

        document.getElementById(row).style["background-color"] = '#94cdfe';
    }
    document.getElementById(chboxname).checked = !document.getElementById(chboxname).checked;    
}


function pageChanged(list, diff) {

    var nodes = document.getElementById(list).children;

    Array.from(nodes).forEach(li => {
        if (li.getAttribute("type") === "text" && li.getAttribute("name") === "nums") {
            try {

                var num = li.getAttribute("value");

                var chboxname = (diff + 'chb ' + num);
                var row = (diff + 'row ' + num);

                document.getElementById(row).style["background-color"] = '#94cdfe';
                document.getElementById(chboxname).checked = 'checked';

            } catch { }
        }
    });
}

function submitForm(list, action, req_num) {

    var form = document.getElementById(list);

    if (form.childElementCount >= req_num) {
        document.getElementById('error_text').style['opacity'] = 0;
        form.setAttribute("action", action);
        form.submit();
    }
    else {
        document.getElementById('error_text').style['opacity'] = 1;
    }
}

function copyValue(src, dest) {
    var srcVal = document.getElementById(src).value;
    document.getElementById(dest).value = srcVal;
}

function toggleDisplay(id, statment_1, statment_2) {
    var el = document.getElementById(id);
    if (el.style['display'] === statment_1)
        el.style['display'] = statment_2;
    else
        el.style['display'] = statment_1;
}



function resizableGrid(table) {
    var row = table.getElementsByTagName('tr')[0],
        cols = row ? row.children : undefined;
    //alert(table.outerHTML)
    if (!cols) return;
    for (var i = 0; i < cols.length; i++) {
        
        var div = createDiv(table.offsetHeight);
        cols[i].appendChild(div);
        cols[i].style.position = 'relative';
        setListeners(div);
    }
}

function createDiv(height) {
    var div = document.createElement('div');
    div.style.top = 0;
    div.style.right = 0;
    div.style.width = '5px';
    div.style.position = 'absolute';
    div.style.cursor = 'col-resize';
    div.style.userSelect = 'none';
    div.style.height = height + 'px';
    div.className = 'columnSelector';
    return div;
}

function setListeners(div) {
    var pageX, curCol, nxtCol, curColWidth, nxtColWidth;
    div.addEventListener('mousedown', function (e) {
        curCol = e.target.parentElement;
        nxtCol = curCol.nextElementSibling;
        pageX = e.pageX;
        curColWidth = curCol.offsetWidth
        if (nxtCol)
            nxtColWidth = nxtCol.offsetWidth
    });

    document.addEventListener('mousemove', function (e) {
        if (curCol) {
            var diffX = e.pageX - pageX;

            if (nxtCol)
                nxtCol.style.width = (nxtColWidth - (diffX)) + 'px';

            curCol.style.width = (curColWidth + diffX) + 'px';
        }
    });

    document.addEventListener('mouseup', function (e) {
        curCol = undefined;
        nxtCol = undefined;
        pageX = undefined;
        nxtColWidth = undefined;
        curColWidth = undefined;
    });
}













