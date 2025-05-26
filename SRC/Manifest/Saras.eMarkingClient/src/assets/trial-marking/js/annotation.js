//--------Global variables--------------------
var previouslyClicked = null;
var maincanvas, maincontext, tempcanvas, tempcontext, drawcanvas, drawcontext;
var linetempcanvas, linetempcontext;
var maincanvasId = '#mainCanvas';
var drawcanvasId = '#drawCanvas';
var IsFirst = true;
var IsStarted = false;
var IsEraser = false;
var tool;

var tool_default = "";
var color = "";
var fontsize = "24px";
var symbolURL = "";
var assignedmarks = "";
var ShowTextarea = false;
var temp_x, temp_y;
var minx, miny, maxx, maxy, hgt, wdt;
var hdnObjectCount = '#hdnObjectCount';
var hdnURL = '#hdnURL';
var hdnURLOut = '#hdnURLOut';

var radius = 0;
var errmsg = "";
var ParentId = -1;
//Main Canvas on which essay text will be rendered
var maincanvaselement = document.createElement("canvas");
var WIDTH = 0;
var HEIGHT = 0;
var IsFirstDraw = true;
var isReady = true;
var drpVal = 1.00;
var isMaginify = false;
var isSeletedMove = false;

//This will tell to load direct text in main canvas or load already saved image of evaluated answer
// @ts-ignore: Object is possibly 'null'.



var isautosave;
var IsImageLoadingToMainCanvas = true;
if (funGetManuScript() == "1" && !IsImageLoadingToMainCanvas) {
    var rows = parseInt((funGetManuScriptRowsColumns()).split(';')[0]);
    if (rows <= 20)
        HEIGHT = 1175;
    if (rows == 40)
        HEIGHT = 2250;
    if (rows == 50)
        HEIGHT = 2800;
    if (rows == 60)
        HEIGHT = 3325;
}

//For drawing objects on temp canvas
var drawnObject = []; //boxes2
var selectionHandles = [];
var INTERVAL = 20;
var isDrag = false;
var isResizeDrag = false;
var expectResize = -1;
var canvasValid = false;
var mySel = null;
var mySelColor = '#CC0000';
var mySelWidth = 2;
var mySelBoxColor = 'darkred';
var mySelBoxSize = 6;
var stylePaddingLeft, stylePaddingTop, styleBorderLeft, styleBorderTop;
var myTable = '';
function annotateimg(pimg, templet) {
    
    imgload();
    sLoadImage(pimg);
}
function Loadimg(pimg, hdraw, cmts, isauto, colr) {
    
  
    document.getElementById("Pen").style.color = colr;
    
    isautosave = isauto ;
    if(isauto)
    {
        ClearAllNR();
    }
    else
    {
        ClearAll();
    }

   
    $("#txtAnnotate").val(cmts);
    sLoadImage(pimg, hdraw, colr);
    $(document).on({
        "contextmenu": function (e) {
           
             rightclick(e);
    
            // Stop the context menu
            e.preventDefault();
        },
        "click": function (e) {
            var tmptxt = document.getElementById("rightclick");
       if(tmptxt != null)
       {
        tmptxt.remove();
        
        }
        }
        
        
    }); 
   
   
}

// Implementation of manuscript
function imgload() {
    
    try {
        if (!isReady)
            return false;

        $('#tools').text("Tools");
        $('#dvColor').text("Color");
        $('#optMedium').text("Medium");
        $('#optLarge').text("Large");
        $('#optXlarge').text("X-Large");
        $('#optXxlarge').text("XX-Large");
        $('#dvFontsize').text("Font size");
        $('#dvnumbering').text("Numbering");
        $('#optSelectdrop').text("<--Select-->");
        $('#btnclearall').prop('title', "Clear all");
        $('#btnclearall').prop('value', "Clear all");


        //Creating canvas element inside maincanvas            
        maincanvaselement.id = "mainCanvas";

        document.getElementById("container").appendChild(maincanvaselement);
        maincanvas = document.getElementById('mainCanvas');
        $(hdnObjectCount).val(0);
        if (!maincanvas) {
            alert("Could not load canvas, Please use the HTML supported browser");

            return;
        }
        if (!maincanvas.getContext) {
            alert("Could not load canvas context, Please use the HTML supported browse");

            return;
        }
        // Get the 2D canvas context.   
        maincontext = maincanvas.getContext('2d');
        if (!maincontext) {
            alert("Could not load canvas context, Please use the HTML supported browse");

            return;
        }

        if (!IsImageLoadingToMainCanvas) {
            //Calculating No Of Lines to calculate canvas height

            WIDTH = $('.containerDiv').width() - 10; // Size of the main div inside which canvas is rendered

            var lineHeight = 25; //Line hight in canvas
            var x = 10; //Writing text top margin
            var y = 20; //Writing text left margin
            var NoOfLines = 0; //To keep the no of lines count for the given text
            var text = ""; //Getting student answer from parent page hidden variable        
            var word = text.split(' ');
            var line = '';
            //This loop will calculate the no of lines need to represent given text
            for (var n = 0; n < word.length; n++) {
                var testLine = line + word[n] + ' ';
                var metrics = maincontext.measureText(testLine);
                var testWidth = metrics.width;
                if ((testWidth > (WIDTH - 10)) && n > 0) {
                    maincontext.fillText(line, x, y);
                    line = word[n] + ' ';
                    y += parseFloat(lineHeight);
                    NoOfLines += 1;
                }
                else {
                    line = testLine;
                }
            }


            if (HEIGHT < $('#container').height())
                HEIGHT = $('#container').height() - 2;
            maincanvas.height = HEIGHT;
            maincanvas.width = WIDTH;
        }

        // Add the draw temporary canvas.
        var containr = maincanvas.parentNode;
        drawcanvas = document.createElement('canvas');
        if (!drawcanvas) {

            return;
        }
        drawcanvas.id = 'drawCanvas';

        drawcanvas.width = $('#container').width();
        drawcanvas.height = HEIGHT;
        containr.appendChild(drawcanvas);
        drawcontext = drawcanvas.getContext('2d');
        // Add the temporary canvas.        

        tempcanvas = document.createElement('canvas');
        if (!tempcanvas) {

            return;
        }
        tempcanvas.id = 'tempCanvas';
        tempcanvas.width = WIDTH;
        tempcanvas.height = HEIGHT;
        tempcontext = tempcanvas.getContext('2d');
        if (tool_default != "") {
            if (tools[tool_default]) {
                tool = new tools[tool_default]();
            }
        }

        //Loading image of essay answer written by user to main canvas        

        if (IsImageLoadingToMainCanvas) {

            if (getViewOrEdit() == "2")
                DisableTool(); //Temporary disabled
        }

        sLoadImage();

        drawcanvas.onselectstart = function () { return false; }
        if (document.defaultView && document.defaultView.getComputedStyle) {
            stylePaddingLeft = parseInt(document.defaultView.getComputedStyle(tempcanvas, null)['paddingLeft'], 10) || 0;
            stylePaddingTop = parseInt(document.defaultView.getComputedStyle(tempcanvas, null)['paddingTop'], 10) || 0;
            styleBorderLeft = parseInt(document.defaultView.getComputedStyle(tempcanvas, null)['borderLeftWidth'], 10) || 0;
            styleBorderTop = parseInt(document.defaultView.getComputedStyle(tempcanvas, null)['borderTopWidth'], 10) || 0;
        }
        setInterval(mainDraw, INTERVAL);
        for (var i = 0; i < 8; i++) {
            var rect = new drawObject;
            selectionHandles.push(rect);
        }
    }
    catch (ex) { }
}


function DisableTool() {
    
    $('a[onclick="ToolSelected(this,event);return false"]').attr("disabled", "disabled");
    $('a[onclick="ChangeSize(this,event);return false"]').attr("disabled", "disabled");
   
    $('a[onclick="SymbolSelected(this,event);return false"]').attr("disabled", "disabled");
    $('a[onclick="ClearAllManual(event);return false"]').attr("disabled", "disabled");
    $('#txtAnnotate').attr("disabled", "disabled");

     $('a[onclick="ToolSelected(this,event);return false"]').removeAttr('onclick');
     $('a[onclick="ChangeSize(this,event);return false"]').removeAttr('onclick');
    
     $('a[onclick="SymbolSelected(this,event);return false"]').removeAttr('onclick');
     $('a[onclick="ClearAllManual(event);return false"]').removeAttr('onclick');
 
   
}


function renderText(context, text, x, y, maxWidth, lineHeight, colorr, sfontsize) {

    var line = '';
    if (colorr != null && colorr != "") {
        context.font = "12pt" + ' Arial';
        context.fillStyle = colorr;
    }
    else {
        context.font = '12pt Arial';
        context.fillStyle = '#333';
    }
    var sLineText = text.split('\n');
    for (var h = 0; h < sLineText.length; h++) {
        var words = sLineText[h].split(' ');
        for (var n = 0; n < words.length; n++) {
            var testLine = line + words[n] + ' ';
            var metrics = context.measureText(testLine);
            var testWidth = metrics.width;
            if (testWidth > (maxWidth - 10) && n > 0) {
                context.fillText(line, x, y);
                line = words[n] + ' ';
                y += parseFloat(lineHeight);
            }
            else {
                line = testLine;
            }
        }
        context.fillText(line, x, y);
        line = '';
        y += parseFloat(lineHeight);
    }

}

color = "";
function LoadImage(pimg, colr) {
    
    if(pimg != null && pimg != "" )
    {
        color = colr;
        var imageObj = new Image();
        imageObj.setAttribute('crossOrigin', 'anonymous');
    
        imageObj.src = pimg;
        imageObj.addEventListener('load', function () {
    
            WIDTH = imageObj.width + 100;
    
            HEIGHT = imageObj.height + 180;
    
            $('#mainDiv').width(100 + "%");
            maincanvas.width = (WIDTH * parseFloat(drpVal));
            maincanvas.height = (HEIGHT * parseFloat(drpVal));
    
            drawcanvas.width = (WIDTH * parseFloat(drpVal));
    
            drawcanvas.height = (HEIGHT * parseFloat(drpVal));
            tempcanvas.width = (WIDTH * parseFloat(drpVal));
            tempcanvas.height = (HEIGHT * parseFloat(drpVal));
            if (linetempcanvas != null) {
                linetempcanvas.width = (WIDTH * parseFloat(drpVal));
                linetempcanvas.height = (HEIGHT * parseFloat(drpVal));
            }
            maincontext.scale(drpVal, drpVal);
    
            maincontext.drawImage(imageObj, 0, 0);
            
    
            return false;
        }, false);

    }
   
  
}

//Tool is selected
function ToolSelected(obj, ev) {
    
    $('.selected').removeClass("selected");
    $('.selected').removeClass("selectedicon");
   
    $('#mainCanvas').hide = true;
    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');
    $('.preview').css('background-image', 'url(' + '' + ')');
    try {
        var isSelect = false;
        var isDrawLine = "";
        if (isMaginify) {
            if (drpVal != 1.00) {
                drpVal = 1.00;
                ReDraw(true, false);
            }
            EnableZoom(false);
        }
        isDrawLine = $(obj)[0].attributes["subject"].value;
        RemoveLineCanvas();
        validatePencilWH();
        if (previouslyClicked == null) {
            previouslyClicked = obj;
            $(obj)[0].attributes["IsClicked"].value = "1";
            $(obj).attr("class", "selectedicon");
            isSelect = true;
            if (isDrawLine == "drawLine" || isDrawLine == "drawLineH" || isDrawLine == "drawRectangle" || isDrawLine == "drawSquare" || isDrawLine == "drawOval" || isDrawLine == "drawCircle" || isDrawLine == "Pencil" || isDrawLine == "drawLineArrow" || isDrawLine == "ConnectorNoArrow" || isDrawLine == "ConnectorArrow" || isDrawLine == "drawWavyLineH" || isDrawLine == "drawWavyLineV") {
                linetempcanvas = document.createElement('canvas');
                var contain = maincanvas.parentNode;
                linetempcanvas.id = 'linetempCanvas';

                linetempcanvas.width = $(maincanvasId).width();
                linetempcanvas.height = $(maincanvasId).height();
                contain.appendChild(linetempcanvas);
                linetempcontext = linetempcanvas.getContext('2d');
                $('#color').show();

                if ($(obj)[0].attributes["icontype"].value == "image") {
                    symbolURL = $(obj)[0].children[0].attributes["src"].value;
                    $('.preview').css('background-image', 'url(' + symbolURL + ')');
                }
                else if ($(obj)[0].attributes["icontype"].value == "class") {
                    $('.preview').empty();
                    var chldi = $(obj)[0].innerHTML;
                    $('.preview').append(chldi);

                }


                $('.preview')[0].style.border = "1px solid " + color;
                $('.toolsDiv_prv_maginfy').removeClass('toolsDiv_prv_maginfy').addClass('toolsDiv_prv_color');
                $('.toolsDiv_maginfy').removeClass('toolsDiv_maginfy').addClass('toolsDiv');
            }
            else if (isDrawLine == "symbol") {

                $('#color').hide();

                if ($(obj)[0].attributes["icontype"].value == "image") {
                    symbolURL = $(obj)[0].children[0].attributes["src"].value;
                    $('.preview').css('background-image', 'url(' + symbolURL + ')');
                }
                else if ($(obj)[0].attributes["icontype"].value == "class") {
                    $('.preview').empty();
                    var chld = $(obj)[0].innerHTML;
                    $('.preview').append(chld);

                }
                $('.preview')[0].style.border = "";
                $('.toolsDiv_prv_maginfy').removeClass('toolsDiv_prv_maginfy').addClass('toolsDiv_prv_color');
                $('.toolsDiv_maginfy').removeClass('toolsDiv_maginfy').addClass('toolsDiv');
            }
            else {
                if ($(obj)[0].attributes["icontype"].value == "image") {
                    symbolURL = $(obj)[0].children[0].attributes["src"].value;
                    $('.preview').css('background-image', 'url(' + symbolURL + ')');
                }
                else if ($(obj)[0].attributes["icontype"].value == "class") {
                    $('.preview').empty();
                    var chl = $(obj)[0].innerHTML;
                    $('.preview').append(chl);

                }

                $('#color').show();
                if (isDrawLine == "drawSelectMove")
                    $('.preview')[0].style.border = "";
                else
                    $('.preview')[0].style.border = "1px solid " + color;


            }
        }
        else if ($(previouslyClicked)[0].attributes["IsClicked"].value == "1") {
            $(previouslyClicked)[0].attributes["IsClicked"].value = "0";
            $(previouslyClicked).removeAttr("class");
            if (previouslyClicked != obj) {
                previouslyClicked = obj
                $(obj)[0].attributes["IsClicked"].value = "1";
                $(obj).attr("class", "selectedicon");
                isSelect = true;
                if (isDrawLine == "symbol") {

                    $('#color').hide();

                    if ($(obj)[0].attributes["icontype"].value == "image") {
                        symbolURL = $(obj)[0].children[0].attributes["src"].value;
                        $('.preview').css('background-image', 'url(' + symbolURL + ')');
                    }
                    else if ($(obj)[0].attributes["icontype"].value == "class") {
                        $('.preview').empty();
                        var chlid = $(obj)[0].innerHTML;
                        $('.preview').append(chlid);

                    }


                    $('.preview')[0].style.border = "";
                    $('.toolsDiv_prv_maginfy').removeClass('toolsDiv_prv_maginfy').addClass('toolsDiv_prv_color');
                    $('.toolsDiv_maginfy').removeClass('toolsDiv_maginfy').addClass('toolsDiv');
                }
                else if (isDrawLine == "drawLine" || isDrawLine == "drawLineH" || isDrawLine == "drawRectangle" || isDrawLine == "drawSquare" || isDrawLine == "drawOval" || isDrawLine == "drawCircle" || isDrawLine == "Pencil" || isDrawLine == "drawLineArrow" || isDrawLine == "ConnectorNoArrow" || isDrawLine == "ConnectorArrow" || isDrawLine == "drawWavyLineH" || isDrawLine == "drawWavyLineV") {
                    $('#color').show();

                    if ($(obj)[0].attributes["icontype"].value == "image") {
                        symbolURL = $(obj)[0].children[0].attributes["src"].value;
                        $('.preview').css('background-image', 'url(' + symbolURL + ')');
                    }
                    else if ($(obj)[0].attributes["icontype"].value == "class") {
                        $('.preview').empty();
                        var child = $(obj)[0].innerHTML;
                        $('.preview').append(child);

                    }
                    $('.preview')[0].style.border = "1px solid " + color;
                    linetempcanvas = document.createElement('canvas');
                    var contai = maincanvas.parentNode;
                    linetempcanvas.id = 'linetempCanvas';

                    linetempcanvas.width = $(maincanvasId).width();
                    linetempcanvas.height = $(maincanvasId).height();
                    contai.appendChild(linetempcanvas);
                    linetempcontext = linetempcanvas.getContext('2d');
                    $('.toolsDiv_prv_maginfy').removeClass('toolsDiv_prv_maginfy').addClass('toolsDiv_prv_color');
                    $('.toolsDiv_maginfy').removeClass('toolsDiv_maginfy').addClass('toolsDiv');
                }
                else {
                    if ($(obj)[0].attributes["icontype"].value == "image") {
                        symbolURL = $(obj)[0].children[0].attributes["src"].value;
                        $('.preview').css('background-image', 'url(' + symbolURL + ')');
                    }
                    else if ($(obj)[0].attributes["icontype"].value == "class") {
                        $('.preview').empty();
                        var chldn = $(obj)[0].innerHTML;
                        $('.preview').append(chldn);

                    }

                    if (isDrawLine == "drawSelectMove")
                        $('.preview')[0].style.border = "";
                    else
                        $('.preview')[0].style.border = "1px solid " + color



                    $('#color').show();
                    if (isDrawLine == "Magnifying") {
                        EnableZoom(true);
                        $('.toolsDiv_prv_color').removeClass('toolsDiv_prv_color').addClass('toolsDiv_prv_maginfy');
                        $('.toolsDiv').removeClass('toolsDiv').addClass('toolsDiv_maginfy');
                    }
                }
            }
            else {
                previouslyClicked = null;

                $('.preview').empty();
                $('.preview').css('background-image', 'url(' + '' + ')');
                $('.preview')[0].style.border = "";
            }

        }
        UpdateAndRemoveTextArea();
        
        SetEventListener(isSelect, obj, color);
        
        selectmousedown(ev);
        if (isDrawLine != null && isDrawLine == "drawSelectMove")
            isSeletedMove = true;
        else
            isSeletedMove = false;
    }
    catch (e) {
        alert(e.message);
    }
    return false;
}
function RemoveLineCanvas() {

    if (linetempcanvas != null) {
        drawcontext.drawImage(linetempcanvas, 0, 0);
        clear(linetempcontext);
        linetempcanvas.onmousedown = "";
        linetempcanvas.onmousemove = "";
        linetempcanvas.onmouseup = "";
        $('#' + linetempcanvas.id).remove();
        linetempcanvas = null;
    }
}
//add or remove event listener
var symbolObj = null;
var subject = "";
color = "";
function SetEventListener(isselect, object, colr) {
    
    color = colr;
   
    subject = $(object)[0].attributes["subject"].value;
    
    if (isselect) {

        switch (subject) {

            case "drawSelectMove":
                symbolObj = subject;
                drawcanvas.onmousedown = selectmousedown;
                drawcanvas.onmousemove = selectmousemove;
                drawcanvas.onmouseup = selectmouseup;
                drawcanvas.onclick = "";
                break;
            case "Magnifying":
                symbolObj = null;
                drawcanvas.onclick = "";
                break;
            case "drawLine":
            case "drawLineH":
            case "Pencil":
            case "drawLineArrow":
            case "ConnectorNoArrow":
            case "ConnectorArrow":
            case "drawWavyLineH":
            case "drawWavyLineV":
                linetempcanvas.onmousedown = linemousedown;
                linetempcanvas.onmousemove = linemousemove;
                linetempcanvas.onmouseup = linemouseup;
                linetempcanvas.ondblclick = linemouseDBClick;
                linetempcanvas.addEventListener('touchstart', linemousedown, false);
                linetempcanvas.addEventListener('touchmove', linemousemove, false);
                linetempcanvas.addEventListener('touchend', linemouseup, false);
                linetempcanvas.onclick = "";
                drawcanvas.onmousedown = "";
                drawcanvas.onmousemove = "";
                drawcanvas.onmouseup = "";
                break;
            case "drawRectangle":
            case "drawSquare":
            case "drawOval":
                linetempcanvas.onmousedown = rectanglemousedown;
                linetempcanvas.onmousemove = rectanglemousemove;
                linetempcanvas.onmouseup = rectanglemouseup;
                linetempcanvas.onclick = "";
                drawcanvas.onmousedown = "";
                drawcanvas.onmousemove = "";
                drawcanvas.onmouseup = "";
                break;
            case "drawCircle":
                linetempcanvas.onmousedown = rectanglemousedown;
                linetempcanvas.onmousemove = circlemousemove;
                linetempcanvas.onmouseup = circlemouseup;
                linetempcanvas.onclick = "";
                drawcanvas.onmousedown = "";
                drawcanvas.onmousemove = "";
                drawcanvas.onmouseup = "";
                break;
            case "drawText":
                drawcanvas.onclick = textclick;
                drawcanvas.onmousedown = "";
                drawcanvas.onmousemove = "";
                drawcanvas.onmouseup = "";
                break;
            case "symbol":
                drawcanvas.onclick = symbolclick;
                drawcanvas.onmousedown = "";
                drawcanvas.onmousemove = "";
                drawcanvas.onmouseup = "";
                break;

            default:
                break;
        }
    }
    else {
        drawcanvas.onmousedown = "";
        drawcanvas.onmousemove = "";
        drawcanvas.onmouseup = "";
        drawcanvas.onclick = "";
    }

}
function draw_canvas(ev) {

    ev._x = ev.offsetX;
    ev._y = ev.offsetY;

}

var lineStrokeType = '';
var offsetx;
var offsety;
function selectmousedown(ev) {
   
    draw_canvas(ev);
    //we are over a selection box
    if (expectResize !== -1) {

        isResizeDrag = true;
        return;
    }
    clear(tempcontext);
    var l = drawnObject.length;

    if (isSeletedMove) {
        for (var i = (l - 1); i >= 0; i--) {
            // draw shape onto draw context
            drawnObject[i].draw(tempcontext);
            //
            // get image data at the mouse x,y pixel
            var imageData = tempcontext.getImageData(ev._x, ev._y, 1, 1);


            if (imageData.data[3] > 0) {

                mySel = drawnObject[i];
                offsetx = ev._x - mySel.x;
                offsety = ev._y - mySel.y;
                mySel.x = ev._x - offsetx;
                mySel.y = ev._y - offsety;
                if (mySel.parentId == -1) // Below line code added to disable the drag feature for the connector line, pencil draw and other tools which has parent id > 0
                    isDrag = true;
                invalidate();
                clear(tempcontext);
                if (mySel.tool == "drawLine" || mySel.tool == "drawLineH" || mySel.tool == "drawLineArrow" || mySel.tool == "ConnectorArrow" || mySel.tool == "drawWavyLineV" || mySel.tool == "drawWavyLineH") {
                    if (mySel.x1 <= mySel.x2 && mySel.y1 <= mySel.y2) {
                        lineStrokeType = 'TL-BR'; //Line Stroke Type - TopLeft to BottomRight
                    }
                    else if (mySel.x1 <= mySel.x2 && mySel.y2 <= mySel.y1) {
                        lineStrokeType = 'BL-TR'; //Line Stroke Type - BottomLeft to TopRight
                    }
                    else if (mySel.x2 <= mySel.x1 && mySel.y2 <= mySel.y1) {
                        lineStrokeType = 'BR-TL'; //Line Stroke Type - BottomRight to TopLeft
                    }
                    else if (mySel.x2 <= mySel.x1 && mySel.y1 <= mySel.y2) {
                        lineStrokeType = 'TR-BL'; //Line Stroke Type - TopRight to BottomLeft
                    }

                }
                else {
                    lineStrokeType = 'ImageStroke'; //Image Stroke - TopRight,click - image placement center

                }
                return;
            }
        }
    }
    // havent returned means we have selected nothing
    mySel = null;
    // clear the draw canvas for next time
    clear(tempcontext);
    // invalidate because we might need the selection border to disappear
    invalidate();

}
function selectmousemove(ev) {
    
   
    draw_canvas(ev);

    if (isDrag && mySel!=null ) {
        switch (lineStrokeType) {
            
            case 'TL-BR': //Line Stroke Type - TopLeft to BottomRight
                mySel.x1 = ev._x - offsetx;
                mySel.y1 = ev._y - offsety;
                mySel.x2 = mySel.x1 + mySel.w;
                mySel.y2 = mySel.y1 + mySel.h;
                mySel.x = mySel.x1; mySel.y = mySel.y1;

                break;
            case 'BL-TR': //Line Stroke Type - BottomLeft to TopRight
                mySel.x1 = ev._x - offsetx;
                mySel.y1 = ev._y - offsety + mySel.h;
                mySel.x2 = ev._x - offsetx + mySel.w;
                mySel.y2 = ev._y - offsety;
                mySel.x = mySel.x1; mySel.y = mySel.y2;

                break;
            case 'BR-TL': //Line Stroke Type - BottomRight to TopLeft
                mySel.x1 = ev._x - offsetx + mySel.w;
                mySel.y1 = ev._y - offsety + mySel.h;
                mySel.x2 = ev._x - offsetx;
                mySel.y2 = ev._y - offsety;
                mySel.x = mySel.x2; mySel.y = mySel.y2;

                break;
            case 'TR-BL': //Line Stroke Type - TopRight to BottomLeft
                mySel.x1 = ev._x - offsetx + mySel.w;
                mySel.y1 = ev._y - offsety;
                mySel.x2 = ev._x - offsetx;
                mySel.y2 = ev._y - offsety + mySel.h;
                mySel.x = mySel.x2; mySel.y = mySel.y1;

                break;
            case 'ImageStroke': //Image Stroke - TopRight,click - image placement center                                                         
                mySel.x = ev._x - offsetx;
                mySel.y = ev._y - offsety;
                break;
        }
        if (mySel.tool == 'drawWavyLineH')
            mySel.y = mySel.y - 20;
        if (mySel.tool == 'drawWavyLineV')
            mySel.x = mySel.x - 20;
        if (mySel.tool == 'drawOval') {

            mySel.x1 = mySel.x + parseInt(mySel.w) / 2;
            mySel.y1 = mySel.y + parseInt(mySel.h) / 2;
        }
        invalidate();
    } else if (isResizeDrag) {
        // time ro resize!
        var oldx, oldy;
        //Line Stroke Type
        switch (lineStrokeType) {
            case 'TL-BR': //Line Stroke Type - TopLeft to BottomRight

                oldx = mySel.x1;
                oldy = mySel.y1;
                switch (expectResize) {
                    case 0:
                        mySel.x1 = ev._x;
                        mySel.y1 = ev._y;
                        mySel.w += oldx - ev._x;
                        mySel.h += oldy - ev._y;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.y2 = mySel.y1 + mySel.h;
                        mySel.x = mySel.x1;
                        mySel.y = mySel.y1;
                        break;
                    case 1:
                        mySel.y1 = ev._y;
                        mySel.h += oldy - ev._y;
                        mySel.y2 = mySel.y1 + mySel.h;
                        mySel.y = mySel.y1;
                        break;
                    case 2:
                        mySel.y1 = ev._y;
                        mySel.w = ev._x - oldx;
                        mySel.h += oldy - ev._y;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.y2 = mySel.y1 + mySel.h;
                        mySel.y = mySel.y1;
                        break;
                    case 3:
                        mySel.x1 = ev._x;
                        mySel.w += oldx - ev._x;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.x = mySel.x1;
                        break;
                    case 4:
                        mySel.w = ev._x - oldx;
                        mySel.x2 = mySel.x1 + mySel.w;
                        break;
                    case 5:
                        mySel.x1 = ev._x;
                        mySel.w += oldx - ev._x;
                        mySel.h = ev._y - oldy;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.y2 = mySel.y1 + mySel.h;
                        mySel.x = mySel.x1;
                        break;
                    case 6:
                        mySel.h = ev._y - oldy;
                        mySel.y2 = mySel.y1 + mySel.h;
                        break;
                    case 7:
                        mySel.w = ev._x - oldx;
                        mySel.h = ev._y - oldy;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.y2 = mySel.y1 + mySel.h;
                        break;
                }
                break;
            case 'BL-TR': //Line Stroke Type - BottomLeft to TopRight
                oldx = mySel.x;
                oldy = mySel.y;
                switch (expectResize) {
                    case 0:
                        mySel.x1 = ev._x;
                        mySel.y1 = ev._y + mySel.h;
                        mySel.w += oldx - ev._x;
                        mySel.h += oldy - ev._y;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.y2 = ev._y;
                        mySel.x = ev._x;
                        mySel.y = ev._y;
                        break;
                    case 1:
                        mySel.h += oldy - ev._y;
                        mySel.y1 = ev._y + mySel.h;
                        mySel.y2 = ev._y;
                        mySel.y = ev._y;
                        break;
                    case 2:
                        mySel.w = ev._x - oldx;
                        mySel.h += oldy - ev._y;
                        mySel.y1 = ev._y + mySel.h;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.y2 = ev._y;
                        mySel.y = ev._y;
                        break;
                    case 3:
                        mySel.x1 = ev._x;
                        mySel.w += oldx - ev._x;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.x = ev._x;
                        break;
                    case 4:
                        mySel.w = ev._x - oldx;
                        mySel.x2 = mySel.x1 + mySel.w;
                        break;
                    case 5:
                        mySel.w += oldx - ev._x;
                        mySel.h = ev._y - oldy;
                        mySel.x1 = ev._x;
                        mySel.y1 = ev._y;
                        mySel.x2 = mySel.x1 + mySel.w;
                        mySel.x = ev._x;
                        break;
                    case 6:
                        mySel.h = ev._y - oldy;
                        mySel.y1 = mySel.y2 + mySel.h;
                        break;
                    case 7:
                        mySel.w = ev._x - oldx;
                        mySel.h = ev._y - oldy;
                        mySel.x1 = ev._x - mySel.w;
                        mySel.y1 = ev._y;
                        mySel.x2 = ev._x;
                        mySel.y2 = ev._y - mySel.h;
                        break;
                }
                break;
            case 'BR-TL': //Line Stroke Type - BottomRight to TopLeft
                oldx = mySel.x2;
                oldy = mySel.y2;
                switch (expectResize) {
                    case 0:
                        mySel.x2 = ev._x;
                        mySel.y2 = ev._y;
                        mySel.w += oldx - ev._x;
                        mySel.h += oldy - ev._y;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.y1 = mySel.y2 + mySel.h;
                        mySel.x = mySel.x2;
                        mySel.y = mySel.y2;
                        break;
                    case 1:
                        mySel.y2 = ev._y;
                        mySel.h += oldy - ev._y;
                        mySel.y1 = mySel.y2 + mySel.h;
                        mySel.y = mySel.y2;
                        break;
                    case 2:
                        mySel.y2 = ev._y;
                        mySel.w = ev._x - oldx;
                        mySel.h += oldy - ev._y;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.y1 = mySel.y2 + mySel.h;
                        mySel.y = mySel.y2;
                        break;
                    case 3:
                        mySel.x2 = ev._x;
                        mySel.w += oldx - ev._x;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.x = mySel.x2;
                        break;
                    case 4:
                        mySel.w = ev._x - oldx;
                        mySel.x1 = mySel.x2 + mySel.w;
                        break;
                    case 5:
                        mySel.x2 = ev._x;
                        mySel.w += oldx - ev._x;
                        mySel.h = ev._y - oldy;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.y1 = mySel.y2 + mySel.h;
                        mySel.x = mySel.x2;
                        break;
                    case 6:
                        mySel.h = ev._y - oldy;
                        mySel.y1 = mySel.y2 + mySel.h;
                        break;
                    case 7:
                        mySel.w = ev._x - oldx;
                        mySel.h = ev._y - oldy;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.y1 = mySel.y2 + mySel.h;
                        break;
                }
                break;
            case 'TR-BL': //Line Stroke Type - TopRight to BottomLeft
                oldx = mySel.x;
                oldy = mySel.y;
                switch (expectResize) {
                    case 0:
                        mySel.x2 = ev._x;
                        mySel.y2 = ev._y + mySel.h;
                        mySel.w += oldx - ev._x;
                        mySel.h += oldy - ev._y;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.y1 = ev._y;
                        mySel.x = ev._x;
                        mySel.y = ev._y;
                        break;
                    case 1:
                        mySel.h += oldy - ev._y;
                        mySel.y2 = ev._y + mySel.h;
                        mySel.y1 = ev._y;
                        mySel.y = ev._y;
                        break;
                    case 2:
                        mySel.w = ev._x - oldx;
                        mySel.h += oldy - ev._y;
                        mySel.y2 = ev._y + mySel.h;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.y1 = ev._y;
                        mySel.y = ev._y;
                        break;
                    case 3:
                        mySel.x2 = ev._x;
                        mySel.w += oldx - ev._x;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.x = ev._x;
                        break;
                    case 4:
                        mySel.w = ev._x - oldx;
                        mySel.x1 = mySel.x2 + mySel.w;
                        break;
                    case 5:
                        mySel.w += oldx - ev._x;
                        mySel.h = ev._y - oldy;
                        mySel.x2 = ev._x;
                        mySel.y2 = ev._y;
                        mySel.x1 = mySel.x2 + mySel.w;
                        mySel.x = ev._x;
                        break;
                    case 6:
                        mySel.h = ev._y - oldy;
                        mySel.y2 = mySel.y1 + mySel.h;
                        break;
                    case 7:
                        mySel.w = ev._x - oldx;
                        mySel.h = ev._y - oldy;
                        mySel.x2 = ev._x - mySel.w;
                        mySel.y2 = ev._y;
                        mySel.x1 = ev._x;
                        mySel.y1 = ev._y - mySel.h;
                        break;
                }
                break;
            case 'ImageStroke': //Image Stroke - TopRight,click - image placement center

                oldx = mySel.x;
                oldy = mySel.y;
                switch (expectResize) {
                    case 0:
                        mySel.x = ev._x;
                        mySel.y = ev._y;
                        mySel.w += oldx - ev._x;
                        mySel.h += oldy - ev._y;

                        break;
                    case 1:
                        if (mySel.tool != "drawCircle") {
                            mySel.y = ev._y;
                            mySel.h += oldy - ev._y;
                        }

                        break;
                    case 2:
                        mySel.y = ev._y;
                        mySel.w = ev._x - oldx;
                        mySel.h += oldy - ev._y;

                        break;
                    case 3:
                        if (mySel.tool != "drawCircle") {
                            mySel.x = ev._x;
                            mySel.w += oldx - ev._x;
                        }

                        break;
                    case 4:
                        if (mySel.tool != "drawCircle")
                            mySel.w = ev._x - oldx;

                        break;
                    case 5:
                        mySel.x = ev._x;
                        mySel.w += oldx - ev._x;
                        mySel.h = ev._y - oldy;

                        break;
                    case 6:
                        if (mySel.tool != "drawCircle")
                            mySel.h = ev._y - oldy;

                        break;
                    case 7:
                        mySel.w = ev._x - oldx;
                        mySel.h = ev._y - oldy;

                        break;
                }
                if (mySel.tool == 'drawOval') {
                    mySel.x1 = mySel.x + parseInt(mySel.w) / 2;
                    mySel.y1 = mySel.y + parseInt(mySel.h) / 2;
                }
                if (mySel.tool == 'drawSquare') {
                    if (mySel.w > mySel.h)
                        mySel.h = mySel.w;
                    else
                        mySel.w = mySel.h;
                }
                if (mySel.tool == 'drawCircle') {
                    try {
                        if (expectResize == 3 || expectResize == 4) {
                            if (expectResize == 3) {
                                if (ev._x > oldx)
                                    mySel.radius = parseFloat(mySel.radius - (Math.abs(ev._x - oldx) / 2));
                                else
                                    mySel.radius = parseFloat(mySel.radius + (Math.abs(ev._x - oldx) / 2));
                                mySel.x = ev._x;
                            }
                            else if (expectResize == 4) {
                                mySel.radius = (Math.abs(ev._x - oldx) / 2);
                            }
                            mySel.x1 = (mySel.x + mySel.radius);
                            mySel.y1 = (mySel.y + mySel.radius);
                            mySel.h = mySel.w = (mySel.radius * 2);
                        }
                        else {
                            if (expectResize == 1) {
                                if (ev._y > oldy)
                                    mySel.radius = parseFloat(mySel.radius - (Math.abs(ev._y - oldy) / 2));
                                else
                                    mySel.radius = parseFloat(mySel.radius + (Math.abs(ev._y - oldy) / 2));
                                mySel.y = ev._y;
                            }
                            else if (expectResize == 6) {
                                mySel.radius = (Math.abs(ev._y - oldy) / 2);
                            }
                            mySel.y1 = (mySel.x + mySel.radius);
                            mySel.y1 = (mySel.y + mySel.radius);
                            mySel.h = mySel.w = (mySel.radius * 2);
                        }

                    } catch (e) { alert(e.message); }

                }
                break;
        }
        invalidate();
    }

    if (mySel !== null && !isResizeDrag && mySel.parentId == -1) {
        for (var i = 0; i < 8; i++) {
            var cur = selectionHandles[i];
            if (ev._x >= cur.x && ev._x <= cur.x + mySelBoxSize &&
                ev._y >= cur.y && ev._y <= cur.y + mySelBoxSize) {
                expectResize = i;
                invalidate();

                switch (i) {
                    case 0:
                        if (mySel.tool != "drawCircle" && mySel.tool != "drawWavyLineV" && mySel.tool != "drawWavyLineH")
                            $(drawcanvasId)[0].style.cursor = 'nw-resize';
                        break;
                    case 1:
                        if (mySel.tool != "drawWavyLineH")
                            $(drawcanvasId)[0].style.cursor = 'n-resize';
                        break;
                    case 2:
                        if (mySel.tool != "drawCircle" && mySel.tool != "drawWavyLineV" && mySel.tool != "drawWavyLineH")
                            $(drawcanvasId)[0].style.cursor = 'ne-resize';
                        break;
                    case 3:
                        if (mySel.tool != "drawWavyLineV")
                            $(drawcanvasId)[0].style.cursor = 'w-resize';
                        break;
                    case 4:
                        if (mySel.tool != "drawWavyLineV")
                            $(drawcanvasId)[0].style.cursor = 'e-resize';
                        break;
                    case 5:
                        if (mySel.tool != "drawCircle" && mySel.tool != "drawWavyLineV" && mySel.tool != "drawWavyLineH")
                            $(drawcanvasId)[0].style.cursor = 'sw-resize';
                        break;
                    case 6:
                        if (mySel.tool != "drawWavyLineH")
                            $(drawcanvasId)[0].style.cursor = 's-resize';
                        break;
                    case 7:
                        if (mySel.tool != "drawCircle" && mySel.tool != "drawWavyLineV" && mySel.tool != "drawWavyLineH")
                            $(drawcanvasId)[0].style.cursor = 'se-resize';
                        break;
                }
                return;
            }
        }
        // not over a selection box, return to normal
        isResizeDrag = false;
        expectResize = -1;
        $(drawcanvasId)[0].style.cursor = 'auto';
    }

}
function selectmouseup(ev) {
    
   
    isDrag = false;
    isResizeDrag = false;
    expectResize = -1;
    
}

function canvas_arrow(context, fromx, fromy, tox, toy, colorc) {

    try {
        var headlen = 10; // length of head in pixels
        var dx = tox - fromx;
        var dy = toy - fromy;
        var angle = Math.atan2(dy, dx);

        context.strokeStyle = colorc;
        context.lineWidth = 2;
        context.moveTo(fromx, fromy);
        context.lineTo(tox, toy);
        context.lineTo(tox - headlen * Math.cos(angle - Math.PI / 6), toy - headlen * Math.sin(angle - Math.PI / 6));
        context.moveTo(tox, toy);
        context.lineTo(tox - headlen * Math.cos(angle + Math.PI / 6), toy - headlen * Math.sin(angle + Math.PI / 6));
        context.stroke();
    }
    catch (e) { alert(e.message()); }
}

function linemouseDBClick(ev) {
    
 
    ParentId = -1;
    clear(linetempcontext);
    if (subject == "ConnectorArrow") {
        drawnObject[drawnObject.length - 1].tool = "drawLineArrow";
        canvasValid = false;
        mainDraw();
    }
    temp_x = ""; temp_y = "";

}

function linemousedown(ev) {
    
    
    document.getElementById("getchangeddetails").click();
    draw_canvas(ev);
    if (subject == "Pencil" || subject == "drawLineArrow" || subject == "drawLine" || subject == "drawLineH" || subject == "drawWavyLineH" || subject == "drawWavyLineV") {
        temp_x = ev._x;
        temp_y = ev._y;
        IsStarted = true;
        ParentId = -1;
    }
    else if (subject == "ConnectorNoArrow" || subject == "ConnectorArrow") {
        if (temp_x != null && temp_x != "" && ((ev._x > temp_x ? (ev._x - temp_x) : (temp_x - ev._x)) > 5 || (ev._y > temp_y ? (ev._y - temp_y) : (temp_y - ev._y)) > 5)) {
                       
            addRect("drawLine", temp_x, temp_y, temp_x, temp_y, ev._x, ev._y, 0, 0, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, ParentId);
            //assign the current point to tempx as it is start point now.
            temp_x = ev._x;
            temp_y = ev._y;
        }
        else { //Is first point of connector line
            temp_x = ev._x;
            temp_y = ev._y;
            ParentId = drawnObject.length;
        }
    }

    if (subject == "Pencil") {
        if (drawnObject != null)
            ParentId = drawnObject.length;
        clear(linetempcontext);
        linetempcontext.beginPath();
        linetempcontext.moveTo(temp_x, temp_y);
    }
    ev.preventDefault();

}
function linemousemove(ev) {
    

    draw_canvas(ev);
    if (!IsStarted) {
        return;
    }
    if (subject == "Pencil") {
        linetempcontext.strokeStyle = color;
        linetempcontext.lineWidth = 1;
        linetempcontext.lineTo(ev._x, ev._y);
        linetempcontext.stroke();
        $(hdnObjectCount).val(parseInt($(hdnObjectCount).val()) + 1);
        
        addRect("Pencil", temp_x, temp_y, temp_x, temp_y, ev._x, ev._y, 0, 0, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, ParentId);
        temp_x = ev._x;
        temp_y = ev._y;
        ev.preventDefault();
    }
    else if (subject == "drawWavyLineH" || subject == "drawWavyLineV") {
        clear(linetempcontext);
        linetempcontext.beginPath();
        if (subject == "drawWavyLineH")
            drawWaveLine(linetempcontext, (ev._x < temp_x ? ev._x : temp_x), temp_y, (ev._x > temp_x ? ev._x : temp_x), true, color, 2);
        else
            drawWaveLine(linetempcontext, temp_x, (ev._y < temp_y ? ev._y : temp_y), (ev._y > temp_y ? ev._y : temp_y), false, color, 2);
        linetempcontext.closePath();
        ev.preventDefault();
    }
    else {

        clear(linetempcontext);
        linetempcontext.beginPath();
        linetempcontext.moveTo(temp_x, temp_y);
        linetempcontext.lineTo(ev._x, ev._y);
        linetempcontext.strokeStyle = color;
        linetempcontext.lineWidth = 2;
        linetempcontext.stroke();
        linetempcontext.closePath();
        ev.preventDefault();
    }

}
function linemouseup(ev) {
    
   
    draw_canvas(ev);
    //  
    if (IsStarted) {
        linemousemove(ev);
        IsStarted = false;
    }
    if (subject == "Pencil") {
        linemousemove(ev);
        linetempcontext.closePath();
        lineStrokeType = '';
    }

    minx = Math.min(ev._x, temp_x);
    miny = Math.min(ev._y, temp_y);
    maxx = Math.max(ev._x, temp_x);
    maxy = Math.max(ev._y, temp_y);
    $(hdnObjectCount).val(parseInt($(hdnObjectCount).val()) + 1);
    var x = 0, y = 0;
    if (temp_x <= ev._x && temp_y <= ev._y) {
        x = temp_x; y = temp_y;
    }
    if (temp_x <= ev._x && ev._y <= temp_y) {
        x = temp_x; y = ev._y;
    }
    if (ev._x <= temp_x && ev._y <= temp_y) {
        x = ev._x; y = ev._y;
    }
    if (ev._x < temp_x && temp_y < ev._y) {
        x = ev._x; y = temp_y;
    }

    if (subject == "drawWavyLineH") {
        if ((ev._x > temp_x ? ev._x : temp_x) - (ev._x < temp_x ? ev._x : temp_x) > 20)
            addRect("drawWavyLineH", (ev._x < temp_x ? ev._x : temp_x), temp_y - 20, (ev._x < temp_x ? ev._x : temp_x), temp_y, (ev._x > temp_x ? ev._x : temp_x), temp_y, (maxx - minx), 40, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, ParentId);
    }
    else if (subject == "drawWavyLineV") {
        if ((ev._y > temp_y ? ev._y : temp_y) - (ev._y < temp_y ? ev._y : temp_y) > 20)
            addRect("drawWavyLineV", temp_x - 20, (ev._y < temp_y ? ev._y : temp_y), temp_x, (ev._y < temp_y ? ev._y : temp_y), temp_x, (ev._y > temp_y ? ev._y : temp_y), 40, (maxy - miny), 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, ParentId);
    }

    if (subject == "drawLineArrow" && ((ev._x > temp_x ? (ev._x - temp_x) : (temp_x - ev._x)) > 5 || (ev._y > temp_y ? (ev._y - temp_y) : (temp_y - ev._y)) > 5))
        addRect("drawLineArrow", x, y, temp_x, temp_y, ev._x, ev._y, maxx - minx, maxy - miny, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
    else if (subject != "Pencil" && subject != "drawWavyLineH" && subject != "drawLineH" && subject != "drawWavyLineV" && subject != "drawLineArrow" && subject != "ConnectorNoArrow" && subject != "ConnectorArrow" && ((ev._x > temp_x ? (ev._x - temp_x) : (temp_x - ev._x)) > 5 || (ev._y > temp_y ? (ev._y - temp_y) : (temp_y - ev._y)) > 5))
        addRect("drawLine", x, y, temp_x, temp_y, ev._x, ev._y, maxx - minx, maxy - miny, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
    else if (subject == "drawLineH")
        addRect("drawLineH", (ev._x < temp_x ? ev._x : temp_x) - 5, temp_y, temp_x, temp_y, ev._x, temp_y, (maxx - minx) + 5, 1, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
    if (subject != "ConnectorNoArrow" && subject != "ConnectorArrow") {
        temp_x = ""; temp_y = "";
    }
    clear(linetempcontext);

}
function rectanglemousedown(ev) {
    
   
    document.getElementById("getchangeddetails").click();
    draw_canvas(ev);
    temp_x = ev._x;
    temp_y = ev._y;
    IsStarted = true;
    ev.preventDefault();
    
}
function rectanglemousemove(ev) {
    draw_canvas(ev);
    if (!IsStarted) {
        return;
    }
    minx = Math.min(ev._x, temp_x);
    miny = Math.min(ev._y, temp_y);
    wdt = Math.abs(ev._x - temp_x);
    hgt = Math.abs(ev._y - temp_y);
    clear(linetempcontext);
    linetempcontext.lineWidth = 2;
    if (!wdt || !hgt) {
        return;
    }
    linetempcontext.strokeStyle = color;
    if (subject == "drawOval") {
        linetempcontext.beginPath();
        drawOval(linetempcontext, minx + wdt / 2, miny + hgt / 2, wdt, hgt, color);
        linetempcontext.closePath();
    }
    else if (subject == "drawSquare") {
        if (wdt > hgt)
            linetempcontext.strokeRect(minx, miny, wdt, wdt);
        else
            linetempcontext.strokeRect(minx, miny, hgt, hgt);
    }
    else
        linetempcontext.strokeRect(minx, miny, wdt, hgt);
    ev.preventDefault();
}
function rectanglemouseup(ev) {
    
   
    draw_canvas(ev);
    if (IsStarted) {
        rectanglemousemove(ev);
        IsStarted = false;
    }
    ev.preventDefault();
    $(hdnObjectCount).val(parseInt($(hdnObjectCount).val()) + 1);
    if (subject == "drawOval")
        addRect("drawOval", minx, miny, minx + wdt / 2, miny + hgt / 2, 0, 0, wdt, hgt, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
    else if (subject == "drawSquare") {
        if (wdt > hgt)
            addRect("drawSquare", minx, miny, 0, 0, 0, 0, wdt, wdt, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
        else
            addRect("drawSquare", minx, miny, 0, 0, 0, 0, hgt, hgt, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
    }
    else
        addRect("drawRectangle", minx, miny, 0, 0, 0, 0, wdt, hgt, 0, "", null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
    clear(linetempcontext);
    temp_x = ""; temp_y = "";

}

function circlemousemove(ev) {
    
    
    draw_canvas(ev);
    if (!IsStarted) {
        return;
    }
    clear(linetempcontext);
    minx = (ev._x + temp_x) / 2;
    miny = (ev._y + temp_y) / 2;
    maxx = Math.max(ev._x, temp_x);
    maxy = Math.max(ev._y, temp_y);
    wdt = Math.abs(ev._x - temp_x);
    hgt = Math.abs(ev._y - temp_y);

    radius = Math.max(
        Math.abs(ev._x - temp_x),
        Math.abs(ev._y - temp_y)
    ) / 2;

    linetempcontext.beginPath();
    linetempcontext.lineWidth = 2;
    linetempcontext.strokeStyle = color;
    linetempcontext.arc(minx, miny, radius, 0, Math.PI * 2, false);
    document.getElementById('hndCircleRef').value = minx + " Y:" + miny + " R:" + radius + ":" + 0 + ":" + Math.PI * 2;

    linetempcontext.stroke();
    linetempcontext.closePath();
    ev.preventDefault();
    
}
function circlemouseup(ev) {
   
    draw_canvas(ev);
    if (IsStarted) {
        circlemousemove(ev);
        IsStarted = false;
    }
    ev.preventDefault();
    var startleft = (minx + maxx) / 2 - radius;
    var starttop = (miny + maxy) / 2 - radius;
    $(hdnObjectCount).val(parseInt($(hdnObjectCount).val()) + 1);
    addRect("drawCircle", startleft, starttop, minx + wdt / 2, miny + hgt / 2, 0, 0, radius * 2, radius * 2, 0, "", null, $(hdnObjectCount).val(), color, radius, "", fontsize, -1);
    radius = 0;
    clear(linetempcontext);
    temp_x = ""; temp_y = "";

}

//Pencil tool
function pencilmousedown(ev) {
    try {
        draw_canvas(ev);
        maincontext.beginPath();
        maincontext.moveTo(ev._x, ev._y);
        IsStarted = true;
        ev.preventDefault();
    } catch (e) { alert(e.message); }
}

function pencilmousemove(ev) {
    try {
        draw_canvas(ev);
        if (IsStarted) {
            maincontext.strokeStyle = color;
            maincontext.lineTo(ev._x, ev._y);
            maincontext.stroke();
            ev.preventDefault();
        }
    } catch (e) { alert(e.message); }
}

function pencilmouseup(ev) {
    try {
        draw_canvas(ev);
        if (IsStarted) {
            pencilmousemove(ev)
            maincontext.closePath();
            IsStarted = false;
        }
    } catch (e) { alert(e.message); }
}


//Ends here..
function symbolclick(ev) {
    
   

    if(symbolURL != null)
    {
    
    document.getElementById("getchangeddetails").click();
    
     draw_canvas(ev);
    var width = 34;
    var height = 34;
    var iconObj = new Image();
    iconObj.src = symbolURL;
    iconObj.addEventListener('load', function () {
        //set size for canvas
        width = iconObj.width - 22;
        height = iconObj.height - 19;

        $(hdnObjectCount).val(parseInt($(hdnObjectCount).val()) + 1);
      
        if(assignedmarks != "")
        {
            var mark = document.getElementById("score_assigned").innerHTML;
       if(mark != "")
        {
            if((parseFloat(mark) + parseFloat(assignedmarks) <= parseFloat(document.getElementById("maxmar_score").innerHTML)) && (parseFloat(mark) + parseFloat(assignedmarks) >= 0 ) )
            {
         document.getElementById("score_assigned").innerHTML = parseFloat(mark) + parseFloat(assignedmarks) ;
          addRect("symbol", ev._x - (width / 2), ev._y - (height / 2), 0, 0, 0, 0, width, height, 0, "", iconObj, $(hdnObjectCount).val(), '', 0, symbolURL, fontsize, -1, parseFloat(assignedmarks));       

              }
              else
              {
                  var anclick = document.getElementById("alertannotate");
                  anclick.click();
              }
        }
        else
        {
            if((parseFloat(assignedmarks) <= parseFloat(document.getElementById("maxmar_score").innerHTML))&& (parseFloat(assignedmarks) >= 0))
            {
            document.getElementById("score_assigned").innerHTML = parseFloat(assignedmarks);
            addRect("symbol", ev._x - (width / 2), ev._y - (height / 2), 0, 0, 0, 0, width, height, 0, "", iconObj, $(hdnObjectCount).val(), '', 0, symbolURL, fontsize, -1, parseFloat(assignedmarks));       
          
        }
        else
              {
                var anclick = document.getElementById("alertannotate");
                anclick.click();
              }
        
        }
        
        
    }
    if(assignedmarks == "")
    {
        addRect("symbol", ev._x - (width / 2), ev._y - (height / 2), 0, 0, 0, 0, width, height, 0, "", iconObj, $(hdnObjectCount).val(), '', 0, symbolURL, fontsize, -1, "");
    }
   
    }, false);
}

}
function symbolclickwithtable(ev) {
    $('#totl').text($('#total').val());
    draw_canvas(ev);
    var width = 34;
    var height = 34;
    var iconObj = new Image();
    iconObj.src = symbolURL;
    iconObj.addEventListener('load', function () {
        //set size for canvas
        width = iconObj.width;
        height = iconObj.height;
        $(hdnObjectCount).val(parseInt($(hdnObjectCount).val()) + 1);
        addRect("symbol", ev._x - (width / 2), ev._y - (height / 2), 0, 0, 0, 0, width, height, 0, "", iconObj, $(hdnObjectCount).val(), '', 0, symbolURL, fontsize, -1);
    }, false);
}
function rightclickicons(icon_no) {
    
    var tmptxt = document.getElementById("rightclick");
    if(tmptxt != null)
    {
        tmptxt.remove();
    }
   
   document.getElementById(icon_no.id).click();
}
function textclick(ev) {
    
    
    document.getElementById("getchangeddetails").click();
    draw_canvas(ev);
    if (!ShowTextarea) {
        ShowTextarea = true;
        var containe = maincanvas.parentNode;
        var temptext = document.createElement('textarea');
 
        if (!temptext) {
            showmessage('<%= GetMessages("ERR005")%>', "CssInfoMessage", "FS_messageBox");
            return;
        }
        temptext.id = 'temptext';
       
        containe.appendChild(temptext);
        temptext.style.left = ev._x + 'px';
        temptext.style.top = ev._y + 'px';
        temptext.style.position = "absolute";
        temptext.style.zindex = 999;


        temptext.style.height = "50px";
        temptext.style.width = "150px";
        temptext.style.borderStyle = "dotted";
        temptext.style.fontFamily = " arial";
        temptext.style.color = color;
        fontsize = "12px";
        temptext.style.fontSize = fontsize;
        temptext.focus();
    }
    else {
        UpdateAndRemoveTextArea();
    }

}
function rightclick(ev) {

    if(!isautosave && drawnObject.length > 0 )
    {
    var Selectordet = document.getElementById("Selector");
    if(Selectordet.className != "selectedicon" )
    {
        Selectordet.click();

    }
    
    selectmousedown(ev);
    isSeletedMove = true;
    var tmptxt = document.getElementById("rightclick");
    if(tmptxt != null)
    {
        tmptxt.remove();
    }
    
        var containe = maincanvas.parentNode;
        var temptext = document.createElement('div');
        
        if (!temptext) {
            showmessage('<%= GetMessages("ERR005")%>', "CssInfoMessage", "FS_messageBox");
            return;
        }
     

        temptext.id = 'rightclick';
        temptext.style.backgroundColor = "#bbb4b4";
        temptext.innerHTML = "<button style='border-radius:3px;margin-left:10px;margin-top:9px;width:33px;height:33px' title='Delete Selected' onclick='rightclickicons(eraser)'><span class='icon-erase'  style='background: #f8f9fa;margin-left: -6px;font-size: 30px;'></span></button></br><button style='border-radius:3px;margin-left:10px;margin-top:5px;width:33px;height:33px' onclick='rightclickicons(deleteall)' title='Delete All'><span class='icon-Delete_2'  style='background: #f8f9fa;margin-left: -6px;font-size: 30px;'></span></button>"
        containe.appendChild(temptext);
        temptext.style.left = ev._x + 'px';
        temptext.style.top = ev._y + 'px';
        temptext.style.position = "absolute";
        temptext.style.zindex = 999;


        temptext.style.height = "100px";
        temptext.style.width = "55px";
        temptext.style.borderStyle = "outset";
        temptext.style.fontFamily = " arial";
        temptext.style.color = color;
        fontsize = "12px";
        temptext.style.fontSize = fontsize;
        
       
    }
   
}
function UpdateAndRemoveTextArea() {
    
    $(hdnObjectCount).val(drawnObject.length);
    if (ShowTextarea) {
        ShowTextarea = false;
        if ($('#container textarea').length != 0) {
            if ($.trim($('#container textarea').val()) == "") {

                $('#container textarea').remove();
                return true;
            }
            var text = $.trim($('#container textarea').val());
            text = text.replace(/"/g, '\"')
            
            drawcontext.font = fontsize + " arial";
            drawcontext.textAlign = 'left';
            drawcontext.fillStyle = color;
            var maxWidth = $('#container textarea').width() + 10;
            var maxHeight = $('#container textarea')[0].scrollHeight + 10;
            var lineHeight = parseInt(fontsize.split("px")[0]) + 3;
            var x = parseInt(temptext.style.left.split('px')[0]) - 2;
            var y = parseInt(temptext.style.top.split('px')[0]);
            $('#container textarea').remove();
            renderText(drawcontext, text, x, parseFloat(y) + parseFloat(lineHeight), maxWidth, lineHeight, color, fontsize);
            addRect("drawText", x, y, 0, 0, 0, 0, maxWidth, maxHeight, lineHeight, text, null, $(hdnObjectCount).val(), color, 0, "", fontsize, -1);
        }
    }
}
function ChangeColor(obj) {
    color = $(obj)[0].attributes["title"].value;
    $('.preview')[0].style.border = "1px solid " + color;
}
function ChangeFont(obj) {

    fontsize = $(obj)[0].attributes.value.value;
    $('#dp5').toggle(200, 'linear');
    $('#SelfontName')[0].innerText = $(obj)[0].innerText;
}
function ChangeSize(obj, ev) {
    
    if (isMaginify) {
        if (drpVal != 1.00) {
            drpVal = 1.00;
            ReDraw(true, false);
        }
        EnableZoom(false);
    }
    $('.preview').empty();
    
    symbolURL = $(obj)[0].attributes.value.value;
    $('.preview').css('background-image', 'url(' + symbolURL + ')');
    $('.preview')[0].style.border = "";
    if (previouslyClicked != null) {
        $(previouslyClicked)[0].attributes["IsClicked"].value = "0";
        $(previouslyClicked).removeAttr("class");
        previouslyClicked = null;
    }
    RemoveLineCanvas();
    drawcanvas.onclick = symbolclick;
    drawcanvas.onmousedown = "";
    drawcanvas.onmousemove = "";
    drawcanvas.onmouseup = "";
    
    document.getElementById("ap_6").click()
    var cvId = obj.id;
    var dpId = parseInt(cvId.split("_")[1]);
    $('#'+dpId).toggle(200, 'linear');
    
}
function ChangeSizewithtotal(obj, ev, tlt) {

    var tot = parseInt($('#total').val()) + parseInt(tlt);
    $('#total').val(tot);
    if (isMaginify) {
        if (drpVal != 1.00) {
            drpVal = 1.00;
            ReDraw(true, false);
        }
        EnableZoom(false);
    }
    $('.preview').empty();
    symbolURL = $(obj)[0].attributes.value.value;
    $('.preview').css('background-image', 'url(' + symbolURL + ')');
    $('.preview')[0].style.border = "";
    if (previouslyClicked != null) {
        $(previouslyClicked)[0].attributes["IsClicked"].value = "0";
        $(previouslyClicked).removeAttr("class");
        previouslyClicked = null;
    }
    RemoveLineCanvas();
    drawcanvas.onclick = symbolclickwithtable;
    drawcanvas.onmousedown = "";
    drawcanvas.onmousemove = "";
    drawcanvas.onmouseup = "";
    $('#dp8').toggle(200, 'linear');
}
function SymbolSelected(obj, ev) {

    $('.selected').removeClass("selected");
    $('.selectedicon').removeClass("selectedicon");
    $(obj)[0].className = "selected";
    if (isMaginify) {
        if (drpVal != 1.00) {
            drpVal = 1.00;
            ReDraw(true, false);
        }
        EnableZoom(false);
    }
    $('.preview').empty();

    if($(obj)[0].attributes.marks != undefined)
    {
    assignedmarks = $(obj)[0].attributes.marks.value;
    }
    else
    {
        assignedmarks = "";
    }
    symbolURL = $(obj)[0].attributes.value.value;
    $('.preview').css('background-image', 'url(' + symbolURL + ')');
    $('.preview')[0].style.border = "";
    if (previouslyClicked != null) {
        $(previouslyClicked)[0].attributes["IsClicked"].value = "0";
        $(previouslyClicked).removeAttr("class");
        previouslyClicked = null;
    }
    RemoveLineCanvas();
    drawcanvas.onclick = symbolclick;
    drawcanvas.onmousedown = "";
    drawcanvas.onmousemove = "";
    drawcanvas.onmouseup = "";
   
    $('#dp4').toggle(200, 'linear');
    
    
}

// New methods on the Box class
drawObject.prototype = {
    draw: function (context) {

        if (context == tempcontext) {
            context.fillStyle = color ; // always want black for the draw canvas
        } else {
            context.fillStyle = 'transparent';
        }
        if (this.x > WIDTH || this.y > HEIGHT) return;
        if (this.x + this.w < 0 || this.y + this.h < 0) return;
        switch (this.tool) {
            case "drawSelectMove":
                context.drawImage(this.iconObj, this.x, this.y, this.w, this.h);
                break;
            case "Pencil":
            case "drawLine":
            case "drawLineH":
                context.beginPath();
                context.moveTo(this.x1, this.y1);
                context.lineTo(this.x2, this.y2);
                context.strokeStyle = this.color;
                context.lineWidth = 2;
                context.stroke();
                context.closePath();
                break;
            case "drawLineArrow":
                context.beginPath();
                canvas_arrow(context, this.x1, this.y1, this.x2, this.y2, this.color);
                context.closePath();
                break;
            case "drawWavyLineH":
                context.beginPath();
                drawWaveLine(context, this.x1, this.y1, this.x2, true, this.color, 2);
                context.closePath();
                break;
            case "drawWavyLineV":
                context.beginPath();
                drawWaveLine(context, this.x1, this.y1, this.y2, false, this.color, 2);
                context.closePath();
                break;
            case "drawRectangle":
            case "drawSquare":
            case "drawOval":
                context.lineWidth = 2;
                context.strokeStyle = this.color;
                if (this.tool == "drawOval") {
                    context.beginPath();
                    drawOval(context, this.x1, this.y1, this.w, this.h, this.color);
                    context.closePath();
                }
                else
                    context.strokeRect(this.x, this.y, this.w, this.h);
                break;
            case "drawCircle":
                context.beginPath();
                context.lineWidth = 2;
                context.strokeStyle = this.color;
                var tempxn = (this.x + this.w);
                var tempyn = (this.y + this.h);
                var tempradiusn = Math.max(Math.abs(this.x - tempxn), Math.abs(this.y - tempyn)) / 2;
                context.arc(this.x + this.radius, this.y + this.radius, tempradiusn, 0, Math.PI * 2);
                context.stroke();
                context.closePath();
                break;
            case "drawText":
                renderText(context, this.text, this.x, parseFloat(parseFloat(this.y) + parseFloat(this.lineHeight)), this.w, this.lineHeight, this.color, this.FontSize);
                context.fillStyle = 'transparent';
                break;
            case "symbol":
                context.drawImage(this.iconObj, this.x, this.y, this.w, this.h);
                break;
            default:
                break;
        }
        if (isSeletedMove)
            context.fillRect(this.x, this.y, this.w, this.h);
        if (mySel === this) {
            context.strokeStyle = mySelColor;
            context.lineWidth = mySelWidth;
            context.strokeRect(this.x, this.y, this.w, this.h);
            switch (this.tool) {
                case "drawSelectMove":
                    context.drawImage(this.iconObj, this.x, this.y, this.w, this.h);
                    break;
                case "drawLine":
                case "drawLineH":
                    context.beginPath();
                    context.moveTo(this.x1, this.y1);
                    context.lineTo(this.x2, this.y2);
                    context.strokeStyle = this.color;
                    context.lineWidth = 2;
                    context.stroke();
                    context.closePath();
                    break;
                case "drawLineArrow":
                    context.beginPath();
                    canvas_arrow(context, this.x1, this.y1, this.x2, this.y2, this.color);
                    context.closePath();
                    break;
                case "drawWavyLineH":
                    context.beginPath();
                    drawWaveLine(context, this.x1, this.y1, this.x2, true, this.color, 2);
                    context.closePath();
                    break;
                case "drawWavyLineV":
                    context.beginPath();
                    drawWaveLine(context, this.x1, this.y1, this.y2, false, this.color, 2);
                    context.closePath();
                    break;
                case "drawRectangle":
                case "drawSquare":
                case "drawOval":
                    context.lineWidth = 2;
                    context.strokeStyle = this.color;
                    context.strokeRect(this.x, this.y, this.w, this.h);
                    break;
                case "drawCircle":
                    context.beginPath();
                    context.lineWidth = 2;
                    context.strokeStyle = this.color;
                    var tempxp = (this.x + this.w);
                    var tempyp = (this.y + this.h);
                    var tempradiusp = Math.max(Math.abs(this.x - tempxp), Math.abs(this.y - tempyp)) / 2;
                    context.arc(this.x + this.radius, this.y + this.radius, tempradiusp, 0, Math.PI * 2);
                    context.stroke();
                    context.closePath();
                    break;
                case "drawText":
                    renderText(context, this.text, this.x, parseFloat(parseFloat(this.y) + parseFloat(this.lineHeight)), this.w, this.lineHeight, this.color, this.FontSize);
                    context.fillStyle = 'transparent';
                    break;
                case "symbol":
                    context.drawImage(this.iconObj, this.x, this.y, this.w, this.h);
                    break;
                default:
                    break;
            }
            if (isSeletedMove)
                context.fillRect(this.x, this.y, this.w, this.h);
            if (this.parentId == -1) {
                var half = mySelBoxSize / 2;
                selectionHandles[0].x = this.x - half;
                selectionHandles[0].y = this.y - half;

                selectionHandles[2].x = this.x + this.w - half;
                selectionHandles[2].y = this.y - half;

                selectionHandles[5].x = this.x - half;
                selectionHandles[5].y = this.y + this.h - half;

                selectionHandles[7].x = this.x + this.w - half;
                selectionHandles[7].y = this.y + this.h - half;

                //middle left
                selectionHandles[3].x = this.x - half;
                selectionHandles[3].y = this.y + this.h / 2 - half;

                //middle right
                selectionHandles[4].x = this.x + this.w - half;
                selectionHandles[4].y = this.y + this.h / 2 - half;

                selectionHandles[1].x = this.x + this.w / 2 - half;
                selectionHandles[1].y = this.y - half;

                //bottom left, middle, right
                selectionHandles[6].x = this.x + this.w / 2 - half;
                selectionHandles[6].y = this.y + this.h - half;


                context.fillStyle = mySelBoxColor;
                for (var i = 0; i < 8; i++) {
                    if (sCheckTooltype(i, mySel.tool)) {
                        var cur = selectionHandles[i];
                        if (isSeletedMove)
                            context.fillRect(cur.x, cur.y, mySelBoxSize, mySelBoxSize);
                    }
                }
            }
        }
    }
}

function sCheckTooltype(i, toolName) {
    switch (i) {
        case 0:
        case 2:
        case 5:
        case 7:
            if (toolName != "drawCircle" && toolName != "drawWavyLineV" && toolName != "drawWavyLineH")
                return true;
            break;
        case 1:
        case 6:
            if (toolName != "drawWavyLineH")
                return true;
            break;
        case 3:
        case 4:
            if (toolName != "drawWavyLineV")
                return true;
            break;
    }
    return false;
}

function invalidate() {
    canvasValid = false;
}
        
function addRect(tools, x, y, x1, y1, x2, y2, w, h, lineHgt, text, iconObj, count, colr, radis, iconObjSrc, sFontSize, pencilparentid, vale = "" ) {
   
    if(colr == undefined && tools != "symbol")
    {
        colr = document.getElementById('colorid').innerHTML;
    }
    var rect = new drawObject;
    rect.tool = tools;
    rect.x = x;
    rect.y = y;
    rect.x1 = x1;
    rect.y1 = y1;
    rect.x2 = x2;
    rect.y2 = y2;
    rect.w = w
    rect.h = h;
    rect.color = colr;
    rect.text = text;
    rect.radius = radis;
    rect.lineHeight = lineHgt;
    rect.iconObj = iconObj;
    rect.count = count;
    rect.iconObjSrc = iconObjSrc;
    rect.FontSize = sFontSize;
    rect.parentId = pencilparentid;
    rect.value = vale;
    rect.markedby =  document.getElementById('markedby').innerHTML;
    drawnObject.push(rect);
    invalidate();
}

function validatePencilWH() {
    var k, i;
    var sUpdatexy = false;
    var PId = -1, LX1 = -1, LY1 = -1, LY2 = -1, LX2 = -1
    try {
        for (i = 0; i < drawnObject.length; i++) {
            if (drawnObject[i].text == "" && (drawnObject[i].tool == "Pencil" || (drawnObject[i].parentId > -1 && (drawnObject[i].tool == "drawLine" || drawnObject[i].tool == "drawLineArrow")))) {
                if (PId == drawnObject[i].parentId && drawnObject[i].text == "") {
                    sUpdatexy = true;
                    if (drawnObject[i].x1 < LX1)
                        LX1 = drawnObject[i].x1;
                    if (drawnObject[i].x2 < LX1)
                        LX1 = drawnObject[i].x2;
                    if (drawnObject[i].y1 < LY1)
                        LY1 = drawnObject[i].y1;
                    if (drawnObject[i].y2 < LY1)
                        LY1 = drawnObject[i].y2;

                    if (drawnObject[i].x1 > LX2)
                        LX2 = drawnObject[i].x1;
                    if (drawnObject[i].x2 > LX2)
                        LX2 = drawnObject[i].x2;
                    if (drawnObject[i].y1 > LY2)
                        LY2 = drawnObject[i].y1;
                    if (drawnObject[i].y2 > LY2)
                        LY2 = drawnObject[i].y2;
                }
                else if (PId != drawnObject[i].parentId && PId > -1 && drawnObject[i].text == "") {
                    for (k = PId; k < i; k++) {

                        if (drawnObject[k] != null && drawnObject[i].text == "" && (drawnObject[k].tool == "Pencil" || (drawnObject[i].parentId > -1 && (drawnObject[i].tool == "drawLine" || drawnObject[i].tool == "drawLineArrow")))) {
                            if (LX1 > LX2) {
                                var Pencila = LX1;
                                LX1 = LX2;
                                LX2 = Pencila;
                            }
                            if (LY1 > LY2) {
                                var Pencilb = LY1;
                                LY1 = LY2;
                                LY2 = Pencilb;
                            }

                            drawnObject[k].text = "1" + PId + ";" + LX1 + ";" + LY1 + ";" + LX2 + ";" + LY2;
                            drawnObject[k].x = LX1;
                            drawnObject[k].y = LY1;
                            drawnObject[k].w = (LX2 - LX1);
                            drawnObject[k].h = (LY2 - LY1);
                            drawnObject[k].iconObjSrc = i;
                            sUpdatexy = false;
                        }
                    }
                    LX1 = drawnObject[i].x1;
                    LX2 = drawnObject[i].x2;
                    LY1 = drawnObject[i].y1;
                    LY2 = drawnObject[i].y2;
                    PId = drawnObject[i].parentId;
                  //  i = i - 1
                }
                else if (drawnObject[i].text == "" && (drawnObject[i].tool == "Pencil" || (drawnObject[i].parentId > -1 && (drawnObject[i].tool == "drawLine" || drawnObject[i].tool == "drawLineArrow")))) {

                    PId = drawnObject[i].parentId;
                    LX1 = drawnObject[i].x1;
                    LX2 = drawnObject[i].x2;
                    LY1 = drawnObject[i].y1;
                    LY2 = drawnObject[i].y2;

                   // i -= 1
                }
                //else
                    //i = drawnObject[i].iconObjSrc
            }
            else if (PId > -1) {

                for (k = PId; k <= i; k++) {

                    if (drawnObject[k] != null && drawnObject[i].text == "" && (drawnObject[k].tool == "Pencil" || (drawnObject[k].parentId > -1 && (drawnObject[k].tool == "drawLine" || drawnObject[k].tool == "drawLineArrow")))) {

                        drawnObject[k].text = "2" + PId + ";" + LX1 + ";" + LY1 + ";" + LX2 + ";" + LY2;
                        if (LX1 > LX2) {
                            var Pencilc = LX1;
                            LX1 = LX2;
                            LX2 = Pencilc;
                        }
                        if (LY1 > LY2) {
                            var Pencild = LY1;
                            LY1 = LY2;
                            LY2 = Pencild;
                        }
                        drawnObject[k].x = LX1;
                        drawnObject[k].y = LY1;
                        drawnObject[k].w = (LX2 - LX1);
                        drawnObject[k].h = (LY2 - LY1);
                        drawnObject[k].iconObjSrc = i;
                        sUpdatexy = false;
                    }
                }

                PId = -1;
            }
        }
        if (sUpdatexy && PId > -1) { // to update if only one pencil drawing and that too last one..
            for (k = PId; k <= i; k++) {
                if (drawnObject[k] != null && (drawnObject[k].tool == "Pencil" || (drawnObject[k].parentId > -1 && (drawnObject[k].tool == "drawLine" || drawnObject[k].tool == "drawLineArrow")))) {

                    drawnObject[k].text = "3" + PId + ";" + LX1 + ";" + LY1 + ";" + LX2 + ";" + LY2;
                    if (LX1 > LX2) {
                        var Pencile = LX1;
                        LX1 = LX2;
                        LX2 = Pencile;
                    }
                    if (LY1 > LY2) {
                        var Pencilf = LY1;
                        LY1 = LY2;
                        LY2 = Pencilf;
                    }
                    drawnObject[k].x = LX1;
                    drawnObject[k].y = LY1;
                    drawnObject[k].w = (LX2 - LX1);
                    drawnObject[k].h = (LY2 - LY1);
                    drawnObject[k].iconObjSrc = i;

                }
            }
        }
    }
    catch (e) {
    }
}

//Wave line function
function drawWaveLine(drawCanvasCtx, x, y, m, horizontal, colo, linewidth) {
    var sClkwise = true, mn;
    drawCanvasCtx.beginPath();
    drawCanvasCtx.lineWidth = linewidth;
    drawCanvasCtx.strokeStyle = colo;
    if (horizontal)
        mn = x;
    else
        mn = y;
    for (var i = mn; i <= m - 10; i += 10) {
        drawCanvasCtx.moveTo(x, y);
        if (sClkwise) {
            if (horizontal) {
                drawCanvasCtx.lineTo(x + 3, y + 2.5);
                drawCanvasCtx.moveTo(x + 3, y + 2.5);
                drawCanvasCtx.bezierCurveTo((x + 4), (y + 2.5) + 1, x + 7, (y + 2.5) + 1, x + 7, y + 2.5);
                drawCanvasCtx.moveTo(x + 7, y + 2.5);
                drawCanvasCtx.lineTo(x + 10, y);
            }
            else {
                drawCanvasCtx.lineTo(x - 2.5, y + 3);
                drawCanvasCtx.moveTo(x - 2.5, y + 3);
                drawCanvasCtx.bezierCurveTo((x - 4), (y + 3), (x - 3.5), (y + 7), (x - 2.5), y + 7);
                drawCanvasCtx.moveTo((x - 2.5), y + 7);
                drawCanvasCtx.lineTo(x, y + 10);
            }
            sClkwise = false;
        }
        else {
            if (horizontal) {
                drawCanvasCtx.lineTo(x + 3, y - 2.5);
                drawCanvasCtx.moveTo(x + 3, y - 2.5);
                drawCanvasCtx.bezierCurveTo((x + 4), (y - 2.5) - 1, x + 7, (y - 2.5) - 1, x + 7, y - 2.5);
                drawCanvasCtx.moveTo(x + 7, y - 2.5);
                drawCanvasCtx.lineTo(x + 10, y);
            }
            else {
                drawCanvasCtx.lineTo(x + 2.5, y + 3);
                drawCanvasCtx.moveTo(x + 2.5, y + 3);
                drawCanvasCtx.bezierCurveTo((x + 4), (y + 3), (x + 3.5), (y + 7), (x + 2.5), y + 7);
                drawCanvasCtx.moveTo((x + 2.5), y + 7);
                drawCanvasCtx.lineTo(x, y + 10);
            }
            sClkwise = true;
        }
        if (horizontal)
            x += 10;
        else
            y += 10;
    }
    drawCanvasCtx.stroke();
}

function drawOval(linetempcontex, centerX, centerY, width, height, colors) {
    if (height > width) {
        linetempcontex.moveTo(centerX, centerY - (height / 2)); // A1  
        linetempcontex.bezierCurveTo(centerX + (width / 2), centerY - (height / 2), // C1
            centerX + (width / 2), centerY + (height / 2), // C2
            centerX, centerY + (height / 2)); // A2

        linetempcontex.bezierCurveTo(centerX - (width / 2), centerY + (height / 2), // C3
            centerX - (width / 2), centerY - (height / 2), // C4
            centerX, centerY - (height / 2)); // A1        

    }
    else {
        linetempcontex.moveTo(centerX - (width / 2), centerY); // A1   
        linetempcontex.bezierCurveTo(centerX - (width / 2), centerY + (height / 2),  // C1
            centerX + (width / 2), centerY + (height / 2), // C2
            centerX + (width / 2), centerY); // A2

        linetempcontex.bezierCurveTo(centerX + (width / 2), centerY - (height / 2), // C3
            centerX - (width / 2), centerY - (height / 2), // C4
            centerX - (width / 2), centerY); // A1        
    }

    linetempcontex.strokeStyle = colors;
    linetempcontex.stroke();
}

//wipes the canvas context
function clear(c) {
    c.clearRect(0, 0, WIDTH, HEIGHT);
}
function mainDraw() {
    try {
        if (!isReady)
            return false;
        if (!canvasValid) {
            clear(drawcontext);
            var l = drawnObject.length;
            var i = 0;
            for (i = 0; i < l; i++) {
                try {
                    drawnObject[i].draw(drawcontext);
                }
                catch (ex) { }
            }
            canvasValid = true;
        }
    } catch (e) { alert(e.message); }

}
//Remove Selected
function RemoveSelected(obj, ev) {
    
    var colord = document.getElementById('colorid').innerHTML;
    if (mySel != null) {
        drawcontext.clearRect(mySel.x - 3, mySel.y - 3, mySel.w + 6, mySel.h + 6);
        var l = drawnObject.length;
        if (mySel.tool == "Pencil" || mySel.parentId > -1) {
            for (var k = (l - 1); k >= 0; k--) {
                if (drawnObject[k].parentId == mySel.parentId) {
                 if(drawnObject[k].markedby  ==   document.getElementById('markedby').innerHTML )
                 {
                    drawnObject.splice(k, 1);
                    document.getElementById("getchangeddetails").click();
                 }
                  
                }
            }
        }
        else {
            
            for (var i = 0; i < l; i++) {

                var sacheck = 0;
                if (drawnObject[i].count == mySel.count) {
                    if(drawnObject[i].value != "")
                    {
                    var marks = document.getElementById("score_assigned").innerHTML;
                    var scassigned =  parseFloat(marks) - parseFloat(drawnObject[i].value);
                    if(parseFloat(scassigned) >= 0 && parseFloat(scassigned) <= parseFloat(document.getElementById("maxmar_score").innerHTML) )
                    {
                    document.getElementById("score_assigned").innerHTML = scassigned;
                    if(parseFloat(scassigned) == 0)
                    {
                        document.getElementById("score_assigned").innerHTML = "";
                    }
                    }
                    else
                    {
                        
                        sacheck = 1;
                        var anclick = document.getElementById("alertdeleteannotate");
                      anclick.click();
                    }
                }
                if(sacheck == 0)
                {
                    if(drawnObject[i].markedby  ==   document.getElementById('markedby').innerHTML )
                    {
                        drawnObject.splice(i, 1);
                        document.getElementById("getchangeddetails").click();
                    }
                   
                }
                    selectmousedown(ev);
                    break;
                }
            }
        }
    }
    canvasValid = false;
   
    
    mainDraw();
    
}

function AssignFinalObject() {
    
    var ObjDrawnObject = [];
    for (var i = 0; i < drawnObject.length; i++) {
        var Obj = new drawObject();
        Obj.tool = drawnObject[i].tool;
        Obj.x = drawnObject[i].x;
        Obj.y = drawnObject[i].y;
        Obj.w = drawnObject[i].w;
        Obj.h = drawnObject[i].h;
        Obj.x1 = drawnObject[i].x1;
        Obj.y1 = drawnObject[i].y1;
        Obj.x2 = drawnObject[i].x2;
        Obj.y2 = drawnObject[i].y2;
        Obj.radius = drawnObject[i].radius;
        Obj.color = drawnObject[i].color;
        Obj.text = drawnObject[i].text;
        Obj.lineHeight = drawnObject[i].lineHeight;
        Obj.id = drawnObject[i].id;
        Obj.src = drawnObject[i].src;
        Obj.iconObjSrc = drawnObject[i].iconObjSrc;
        Obj.count = drawnObject[i].count;
        Obj.FontSize = drawnObject[i].FontSize;
        Obj.parentId = drawnObject[i].parentId;
        
       Obj.value = drawnObject[i].value;
        Obj.iconObj = "";
        ObjDrawnObject.push(Obj);
    }
    var drawnObjectString = JSON.stringify(ObjDrawnObject, function (key, value) { return value; });
    AssignDrawObject(drawnObjectString);
}

function SaveDrawPoints() {
    
    if (linetempcanvas != null) {
        drawcontext.scale(1.00, 1.00);
        drawcontext.drawImage(linetempcanvas, 0, 0);
        clear(linetempcontext);
        $('#' + linetempcanvas.id).remove();
        linetempcanvas = null;
    }
    maincontext.drawImage(drawcanvas, 0, 0); //Enable this when marking needs to store as image
    $(hdnURLOut).val("");
    var dataURL = maincanvas.toDataURL();
    $(hdnURL).val(dataURL);
    dataURL = dataURL.replace("data:image/png;base64,", "");

    $(hdnURLOut).val(dataURL);
    AssignFinalObject();
}

function ClearAll() {
 
    $('#hdnDrawobject').val("");
    $('#txtAnnotate').val("");
    symbolURL = null;
    Reselect();
    if (linetempcanvas != null) {
        drawcontext.drawImage(linetempcanvas, 0, 0);
        clear(linetempcontext);
        $('#' + linetempcanvas.id).remove();
        linetempcanvas = null;
    }
    drawcontext.clearRect(0, 0, drawcanvas.width, drawcanvas.height);
    drawnObject.length = 0;
    ReDraw(false, false);
    var Selectordet = document.getElementById("Selector");
    if(Selectordet.className == "selectedicon" )
    {
        Selectordet.click();

    }
    UpdateAndRemoveTextArea();
    
}

function ClearAllNR() {
    
    $('#hdnDrawobject').val("");
    $('#txtAnnotate').val("");
    symbolURL = null;
    Reselect();
    if (linetempcanvas != null) {
        drawcontext.drawImage(linetempcanvas, 0, 0);
        clear(linetempcontext);
        $('#' + linetempcanvas.id).remove();
        linetempcanvas = null;
    }
    drawcontext.clearRect(0, 0, drawcanvas.width, drawcanvas.height);
    drawnObject.length = 0;
    ReDraw(false, false);
    var Selectordet = document.getElementById("Selector");
    if(Selectordet.className == "selectedicon" )
    {
        Selectordet.click();

    }
    UpdateAndRemoveTextArea();
}


function ClearAllManual(discrete) {
      
    var Selectordet = document.getElementById("Selector");
    if(Selectordet.className == "" )
    {
        Selectordet.click();

    }
    document.getElementById("getchangeddetails").click();
    $('#hdnDrawobject').val("");
    $('#txtAnnotate').val("");
    symbolURL = null;
    if(discrete != undefined)
    {
    if(discrete.currentTarget.attributes["data-dis"].value == 'true')
    {
        document.getElementById("score_assigned").innerHTML = "";
    }
    }
    Reselect();
    if (linetempcanvas != null) {
        drawcontext.drawImage(linetempcanvas, 0, 0);
        clear(linetempcontext);
        $('#' + linetempcanvas.id).remove();
        linetempcanvas = null;
    }
    var colord = document.getElementById('colorid').innerHTML;
 
        var l = drawnObject.length;
       
            for (var i = (l - 1); i >= 0; i--) {
                
                if(drawnObject[i].markedby  ==   document.getElementById('markedby').innerHTML )
                 {
                    drawnObject.splice(i, 1);
                    document.getElementById("getchangeddetails").click();
                 }
                  
                
            }
       
  
    var Selectordet = document.getElementById("Selector");
    if(Selectordet.className == "selectedicon" )
    {
        Selectordet.click();

    }
    UpdateAndRemoveTextArea();
    if(typeof hdraw !== 'undefined') 
    {
        if( hdraw != null && hdraw != "")
        {
            $('#hdnDrawobject').val(hdraw);
        }
    }
   
   
    setTimeout(function () { drawPreviousDrawObjects(true); }, 1000);
}


function sLoadImage(pimg, hdraw,colr) {
 if( hdraw != null && hdraw != "")
 {
    $('#hdnDrawobject').val(hdraw);
 }
  
    LoadImage(pimg, colr);
}
function drawpreviousobject(hdraw) {
    $('#hdnDrawobject').val(hdraw);
    setTimeout(function () { drawPreviousDrawObjects(true); }, 1000);
}


//EnableZoom
function EnableZoom(sBool) {
    if (sBool) {
        $("#SelectZoom").css("display", "");
        isMaginify = true;
    }
    else {
        $("#SelectZoom").css("display", "none");
        isMaginify = false;
    }
}

function ChangeZoom(obj) {
    drpVal = $(obj).val();
    ReDraw(false, false);
}

function ReDraw(sDrpSelectZoom, sDrawPrev) {
    if (sDrpSelectZoom)
        $("#SelectZoom").val(1.00);
    LoadImage();
    IsFirstDraw = true;
    if (sDrawPrev)
        drawPreviousDrawObjects(false);
    else
        setTimeout(function () { drawPreviousDrawObjects(false); }, 1000);
}

function drawPreviousDrawObjects(sAssignObjects) {

    try {
        IsFirstDraw = true;
        if (IsFirstDraw) {
            IsFirstDraw = false;
            drawcontext.scale(drpVal, drpVal);
            if (document.getElementById('hdnDrawobject').value != "") {
                if (sAssignObjects) {
                    AssignObjects();
                }
                clear(drawcontext);
                for (var i = 0; i < drawnObject.length; i++) {
                    try { drawnObject[i].draw(drawcontext); }
                    catch (ex) { }
                }
                $(hdnObjectCount).val(drawnObject.length);
            }
        }
        fontsize = "16px";
        setTimeout(function () { selfclickimg(); }, 2500);
    }
    catch (e) { alert(e.message); }
}

function selfclickimg() {
    $("a[subject='remove']").trigger("click");
}


function AssignObjects() {
    
    var ObjDrawnObject = [];
    ObjDrawnObject = JSON.parse(document.getElementById('hdnDrawobject').value);
    for (var i = 0; i < ObjDrawnObject.length; i++) {
        var Obj = new drawObject();
        Obj.tool = ObjDrawnObject[i].tool;
        Obj.x = ObjDrawnObject[i].x;
        Obj.y = ObjDrawnObject[i].y;
        Obj.w = ObjDrawnObject[i].w;
        Obj.h = ObjDrawnObject[i].h;
        Obj.x1 = ObjDrawnObject[i].x1;
        Obj.y1 = ObjDrawnObject[i].y1;
        Obj.x2 = ObjDrawnObject[i].x2;
        Obj.y2 = ObjDrawnObject[i].y2;
        Obj.radius = ObjDrawnObject[i].radius;
        Obj.color = ObjDrawnObject[i].color;
        Obj.text = ObjDrawnObject[i].text;
        Obj.lineHeight = ObjDrawnObject[i].lineHeight;
        Obj.id = ObjDrawnObject[i].id;
        Obj.src = ObjDrawnObject[i].src;
        Obj.iconObjSrc = ObjDrawnObject[i].iconObjSrc;
        Obj.count = ObjDrawnObject[i].count;
        Obj.FontSize = ObjDrawnObject[i].FontSize;
        Obj.parentId = ObjDrawnObject[i].parentId;
        Obj.markedby = ObjDrawnObject[i].markedby;
      Obj.value = ObjDrawnObject[i].value;
        if (ObjDrawnObject[i].iconObjSrc != "") {
            var width = 34;
            var height = 34;
            var iconObj = new Image();
            iconObj.addEventListener('load', function () {
                width = iconObj.width - 22;
                height = iconObj.height - 10;
            }, false);
            Obj.iconObj = iconObj;
            iconObj.src = ObjDrawnObject[i].iconObjSrc;
            //draw image
        }
        else
            Obj.iconObj = null;
        drawnObject.push(Obj);
    }
}



drawnObject = []; //boxes2
var requestPath = location.protocol + "//" +

    window.location.host + "/" +

    window.location.pathname.split('/')[1];

function drawObject() {
    var tmptxt = document.getElementById("rightclick");
    if(tmptxt != null)
    {
        tmptxt.remove();
    }
    this.tool = "";
    this.x = 0;
    this.y = 0;
    this.w = 1; // default width and height?
    this.h = 1;
    this.x1 = 0;
    this.y1 = 0;
    this.x2 = 0;
    this.y2 = 0;
    this.radius = 0;
    this.color = color;
    this.iconObj = null;
    this.text = "";
    this.lineHeight = 0;
    this.id = "";
    this.src = "";
    this.count = 0;
    this.iconObjSrc = "";
    this.FontSize = "16px";
    this.parentId = -1;
    this.value = 0;
    this.markedby = "";
}
function funGetStudentAnswer() {
    return document.getElementById('hndEssayresponse').value;
}
function EssayEvaluationDrawObject(Objresponse) {
    alert("Success: " + Objresponse);
}

function DrawObjectErrorMessage(obj) {
    alert("Failure :  " + obj.responseText);
}
function GetEssayFailureByEssay(objResponse) {
    alert(objResponse);
}



function funGetStudentFormatedAnswer() {
    return document.getElementById("divEssyAnswr").innerHTML;
}
function funGetManuScript() {
    return document.getElementById("hdnManuScript").value;
}

function funGetEssayType() {
    return document.getElementById("hndEssayType").value;
}

function funGetManuScriptRowsColumns() {
    return document.getElementById("hndRowColumn").value;
}

function getViewOrEdit() {
    return document.getElementById("hdnView").value;
}

function funhdnURL() {
    return document.getElementById("hdnURL").value;
}



function funhdnResultPath() {
    return document.getElementById("hdnResultPath").value;

}

function funhdnLocalFilePath() {
    return document.getElementById("hdnLocalFilePath").value;
}
function funhdnURLOut() {

    return document.getElementById("hdnURLOut").value;
}


function AssignDrawObject(DrawObject1) {
    try {
        if (DrawObject1 != null)
            document.getElementById("hdnDrawobject").value = DrawObject1;
    }
    catch (e) { alert(e.message); }
}

function saveAnnotation() {

  
    fnSaveItemAnnotation();
   

}
function fnSaveItemAnnotation(bandid) {
    
   
    if (linetempcanvas != null) {
        drawcontext.scale(1.00, 1.00);
        drawcontext.drawImage(linetempcanvas, 0, 0);
        clear(linetempcontext);
        $('#' + linetempcanvas.id).remove();
        linetempcanvas = null;
    }
    var txtComments = document.getElementById("txtAnnotate").value;
    var ObjAnnotationDetails = {};
    var maincanvasn = document.getElementById('mainCanvas');
    
    var dataURL12 = maincanvasn.toDataURL();
   
    ObjAnnotationDetails["ImagePath"] = "";
    ObjAnnotationDetails["AnnotatedImagePath"] = "";
    ObjAnnotationDetails["GeneralComments"] = txtComments;
    ObjAnnotationDetails["bandid"] = bandid;
    ObjAnnotationDetails["Markings"] = JSON.stringify(drawnObject);
    ObjAnnotationDetails["AnnotatedImagebase64"] = dataURL12;
   
  

    return JSON.stringify(ObjAnnotationDetails);
}
function Reselect() {

    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');
    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');

    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');

    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');

    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');

    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');

    $('#dp1').hide(200, 'linear');
    $('#dp2').hide(200, 'linear');
    $('#dp3').hide(200, 'linear');
    $('#dp4').hide(200, 'linear');
    $('#dp5').hide(200, 'linear');
    $('#dp6').hide(200, 'linear');
    $('#dp8').hide(200, 'linear');

    $('#dvComments').hide(200, 'linear');


}

function changeView(obj) {
    symbolURL = null;
    $('.selected').removeClass("selected");
    $('.selectedicon').removeClass("selectedicon");
    $(obj)[0].className = "selected";
 
    var cvId = obj.id;
    var dpId = parseInt(cvId.split("_")[1]);
    switch (dpId) {
        case 0:

            break;
        case 1:
            $('#dp1').toggle(200, 'linear');
            $('#dp2').hide(200, 'linear');
            $('#dp3').hide(200, 'linear');
            $('#dp4').hide(200, 'linear');
            $('#dp5').hide(200, 'linear');
            $('#dp6').hide(200, 'linear');
            break;
        case 2:
            $('#dp1').hide(200, 'linear');
            $('#dp2').toggle(200, 'linear');
            $('#dp3').hide(200, 'linear');
            $('#dp4').hide(200, 'linear');
            $('#dp5').hide(200, 'linear');
            $('#dp6').hide(200, 'linear');
            break;
        case 3:
            $('#dp1').hide(200, 'linear');
            $('#dp2').hide(200, 'linear');
            $('#dp3').toggle(200, 'linear');
            $('#dp4').hide(200, 'linear');
            $('#dp5').hide(200, 'linear');
            $('#dp6').hide(200, 'linear');
            break;
        case 4:
            $('#dp1').hide(200, 'linear');
            $('#dp2').hide(200, 'linear');
            $('#dp3').hide(200, 'linear');
            $('#dp4').toggle(200, 'linear');
            $('#dp5').hide(200, 'linear');
            $('#dp6').hide(200, 'linear');
            break;
        case 5:
            $('#dp1').hide(200, 'linear');
            $('#dp2').hide(200, 'linear');
            $('#dp3').hide(200, 'linear');
            $('#dp4').hide(200, 'linear');
            $('#dp5').toggle(200, 'linear');
            $('#dp6').hide(200, 'linear');
            break;
        case 6:
            $('#dp1').hide(200, 'linear');
            $('#dp2').hide(200, 'linear');
            $('#dp3').hide(200, 'linear');
            $('#dp4').hide(200, 'linear');
            $('#dp5').hide(200, 'linear');
            $('#dp6').toggle(200, 'linear');
            break;
            
        case 8:
            $('#dp1').hide(200, 'linear');
            $('#dp2').hide(200, 'linear');
            $('#dp3').hide(200, 'linear');
            $('#dp4').hide(200, 'linear');
            $('#dp5').hide(200, 'linear');
            $('#dp6').hide(200, 'linear');
            $('#dp8').toggle(200, 'linear');
            break;
     case 9:
            $('#dp1').hide(200, 'linear');
            $('#dp2').hide(200, 'linear');
            $('#dp3').hide(200, 'linear');
            $('#dp4').hide(200, 'linear');
            $('#dp5').hide(200, 'linear');
            $('#dp6').hide(200, 'linear');
            
            $('#dp9').toggle(200, 'linear');
            break;
        case 7:
            $('#dvComments').toggle(200, 'linear');
            break;

            

    }



}
