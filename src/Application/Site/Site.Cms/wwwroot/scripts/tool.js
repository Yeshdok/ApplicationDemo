var searchOptionsDic = new Object();
var ajaxPro = 0;

//获取元素高度，若高度值不合法返回0
function GetElementHeight(selector) {
    if (!selector) {
        return 0;
    }
    var height = $(selector).outerHeight();
    if (isNaN(height)) {
        height = 0;
    }
    return height;
}

//获取元素宽度，若宽度值不合法返回0
function GetElementWeight(selector) {
    if (!selector) {
        return 0;
    }
    var width = $(selector).outerWidth();
    if (isNaN(width)) {
        width = 0;
    }
    return width;
}


//弹层
function OpenDialogPage(options) {
    var defaultOps = {
        title:'新页面',
        width: "800px",
        height: "500px",
        opacity: 0.5,
        duration: 0,
        background: "#f5f5f5",
        lock: true,
        cancel: true,
        resize: false,
        closeLoading: false,
        ok: function () {
            var iframe = this.iframe.contentWindow;
            if (!iframe.document.body) {
                return false;
            };
            if(iframe.ArtEvent){
            	return iframe.ArtEvent();
            }
            return true;
        }
    };
    defaultOps = $.extend(defaultOps, options);
    artDialog.open(options.url, defaultOps,false);
}

//Ajax全局设置
$.ajaxSetup({
    global: false,
    beforeSend: function (xhr, o) {
        AjaxBeforeSend(xhr, this);
    },
    complete: function () {
        AjaxComplete();
    }
});

function AjaxBeforeSend(xhr, options) {
    ajaxPro++;
    xhr.setRequestHeader("Http-Request-Type", "ajax-request");
    if (options && options.data && (options.data.NotShowLoading || options.data.indexOf("NotShowLoading=true") >= 0)) {

    } else {
        window.ShowLoading();
    }
}

function AjaxComplete() {
    ajaxPro--;
    if (ajaxPro <= 0) {
        window.HideLoading();
    }
}

//表单成功回调
function SuccessCallback(res) {
    window.HideLoading();
    if (!res) {
        return;
    }
    if (res.Success) {
        SuccessMsg(res.Message);
        if (res.SuccessRefresh) {
            window.location.href = window.location.href;
        }
        if (res.SuccessClose) {
            art.dialog.opener.SuccessMsg(res.Message);
            art.dialog.close(true);
        }
        if (res.NeedAuthentication) {
            window.top.RedirectLoginPage();
        }
    } else {
        ErrorMsg(res.Message);
    }
}

//表单失败回调
function FailedCallback(res) {
    window.HideLoading();
    ErrorMsg("数据提交失败");
}

//成功消息
function SuccessMsg(msg) {
    TipMsg(msg, 1);
};

//失败消息
function ErrorMsg(msg) {
    TipMsg(msg, 2);
};

//结果消息
function ResultMsg(res) {
    if (!res) {
        return;
    }
    TipMsg(res.Message, res.Success ? 1 : 2);
    if (!res.success && res.NeedAuthentication) {
        window.top.RedirectLoginPage();
    }
};

//显示消息
function TipMsg(msg, type) {
    var style = "success";
    switch (type) {
        case 2:
            style = "error";
            break;
    }
    $.message({
        message:msg,
        type:style,
        time:'3000'
    });
};

//询问框
function ConfirmMsg(msg, fun) {
    art.dialog.confirm(msg, fun, function () { });
}


//表单验证成功事件
function ValidSuccess(label, element) {
    var eleVal = $(element).val();
    if (eleVal == "") {
        return;
    }
    var tipEle = $('span[data-valmsg-for="' + $(element).attr('name') + '"]');
    tipEle.removeClass("error").removeClass("prompt").removeClass("ajax").addClass("ok form-validate-msg micon").html("");
}

//表单验证失败事件
function ValidError(label, element) {
    var tipEle = $('span[data-valmsg-for="' + $(element).attr('name') + '"]');
    tipEle.removeClass("ok").removeClass("prompt").removeClass("ajax").addClass("error form-validate-msg micon");
    tipEle.html(label.html());
}

//显示Ajax等待框
function ShowLoading() {
    try {
        layer.load(2);
    } catch (e) {

    }
}

//关闭Ajax等待框
function HideLoading() {
    try {
        layer.closeAll('loading');
    } catch (e) {

    }
}


//分页搜索
function PageSearch(options) {
    if (!options) {
        return;
    }
    var defaults = {
        url: '',
        data: { page: 1, pageSize: 20 },
        listEle: "#tabe_page_list",
        pagerEle: "#page-foot",
        selectPage: false,
        callback: undefined,
        init: true,
        showPageNum: true
    };
    var pageListId = !options.listEle ? defaults.listEle : options.listEle;
    var searchOptions = searchOptionsDic[pageListId];
    searchOptions = $.extend(true, {}, defaults, options);
    if (searchOptions.init) {
        searchOptions.data.page = 1;
    }
    if (!searchOptions.url || $.trim(searchOptions.url) == "") {
        return;
    }
    searchOptionsDic[pageListId] = searchOptions;
    $.post(searchOptions.url, searchOptions.data, function (res) {
        var listItemEle = $(searchOptions.listEle);
        var dataValues = JSON.parse(res.Datas);
        ReplaceDataTableData(searchOptions.listEle, dataValues);
        InitDataTableChecked(searchOptions.listEle);
        CreatePageControl(res.TotalCount, searchOptions.data.page, searchOptions.data.pageSize, pageListId);
        if (searchOptions.callback) {
            searchOptions.callback(dataValues);
        }
        $(window).resize();
    })
}

//分页控件点击事件
function PagerBtnSearch(page, pageListId) {
    if (isNaN(page) || page <= 0 || !pageListId || $.trim(pageListId) == "") {
        return;
    }
    var searchOptions = searchOptionsDic[pageListId];
    if (!searchOptions || page == searchOptions.data.page) {
        return;
    }
    var newOptions = $.extend(true, {}, searchOptions, { data: { page: page }, init: false });
    PageSearch(newOptions);
}

//生成分页控件
function CreatePageControl(totalCount, currentPage, pageSize, pageListId) {
    var searchOptions = searchOptionsDic[pageListId];
    $(searchOptions.pagerEle + " .pager-ctrol").remove();
    if (isNaN(totalCount) || totalCount <= 0) {
        $(searchOptions.listEle).parent().addClass("b_b_none");
        return;
    }
    $(searchOptions.listEle).parent().removeClass("b_b_none");
    currentPage = isNaN(currentPage) || currentPage < 1 ? 1 : currentPage;
    pageSize = isNaN(pageSize) || pageSize < 1 ? 1 : pageSize;
    var pageCount = Math.ceil(totalCount / pageSize);
    currentPage = currentPage > pageCount ? pageCount : currentPage;
    var isFirstPage = currentPage == 1;
    var isLastPage = currentPage == pageCount;
    var pagerConClass = searchOptions.selectPage ? "pager-ctrol select_pager" : "pager-ctrol";
    pagerConClass += ' row pd-0 mg-0';
    var pagerCon = GetDivByClass(pagerConClass);
    var pcLeft = GetDivByClass("pc-left col-lg-3 col-md-3 d-md-inline-block d-sm-none d-xs-none pdl-5", pagerCon);
    var pcRight = GetDivByClass("pc-right col-lg-9 col-md-9 col-sm-12 d-inline-block txt-right pd-0", pagerCon);
    var btnUrl = "javascript:void(0)";
    //if (!isFirstPage) {
    var firstBtn = GetLinkByClass(btnUrl, "", pcRight);
    var prevBtn = GetLinkByClass(btnUrl, "", pcRight);
    firstBtn.innerHTML = "首页";
    prevBtn.innerHTML = "上一页";
    if (isFirstPage) {
        firstBtn.className = "dis";
        prevBtn.className = "dis";
    } else {
        firstBtn.onclick = function () {
            PagerBtnSearch(1, pageListId);
        };
        prevBtn.onclick = function () {
            PagerBtnSearch(currentPage - 1, pageListId);
        };
    }
    //}
    if (searchOptions.showPageNum) {
        if (pageCount <= 10) {
            for (var p = 1; p <= pageCount; p++) {
                var btn = GetLinkByClass(btnUrl, currentPage == p ? "cur" : "", pcRight);
                btn.innerHTML = p;
                btn.onclick = function () {
                    var npage = parseInt(this.innerHTML);
                    PagerBtnSearch(npage, pageListId);
                };
            }
        } else if (currentPage <= 5) {
            for (var p = 1; p <= 8; p++) {
                var btn = GetLinkByClass(btnUrl, currentPage == p ? "cur" : "", pcRight);
                btn.innerHTML = p;
                btn.onclick = function () {
                    var npage = parseInt(this.innerHTML);
                    PagerBtnSearch(npage, pageListId);
                };
            }
            var btnPoint = GetLinkByClass(btnUrl, "", pcRight);
            btnPoint.innerHTML = "...";
            btnPoint.onclick = function () {
                PagerBtnSearch(9, pageListId);
            };
            var lastBtn = GetLinkByClass(btnUrl, "", pcRight);
            lastBtn.innerHTML = pageCount;
            lastBtn.onclick = function () {
                var npage = parseInt(this.innerHTML);
                PagerBtnSearch(npage, pageListId);
            };
        } else {
            var firstBtn = GetLinkByClass(btnUrl, "", pcRight);
            firstBtn.innerHTML = "1";
            firstBtn.onclick = function () {
                var npage = parseInt(this.innerHTML);
                PagerBtnSearch(npage, pageListId);
            };
            var btnPoint = GetLinkByClass(btnUrl, "", pcRight);
            btnPoint.innerHTML = "...";
            btnPoint.onclick = function () {
                PagerBtnSearch(2, pageListId);
            };
            var beginPage = currentPage - 3;
            var endPage = (currentPage + 4) > pageCount ? pageCount : currentPage + 3;
            for (var p = beginPage; p <= endPage; p++) {
                var btn = GetLinkByClass(btnUrl, currentPage == p ? "cur" : "", pcRight);
                btn.innerHTML = p;
                btn.onclick = function () {
                    var npage = parseInt(this.innerHTML);
                    PagerBtnSearch(npage, pageListId);
                };
            }
            if (endPage < pageCount) {
                var btnPoint2 = GetLinkByClass(btnUrl, "", pcRight);
                btnPoint2.innerHTML = "...";
                btnPoint2.onclick = function () {
                    PagerBtnSearch(endPage + 1, pageListId);
                };
                var lastBtn = GetLinkByClass(btnUrl, "", pcRight);
                lastBtn.innerHTML = pageCount;
                lastBtn.onclick = function () {
                    PagerBtnSearch(pageCount, pageListId);
                };
            }
        }
    }
    //if (!isLastPage) {
    var nextBtn = GetLinkByClass(btnUrl, "", pcRight);
    var lastBtn = GetLinkByClass(btnUrl, "", pcRight);
    nextBtn.innerHTML = "下一页";
    lastBtn.innerHTML = "末页";
    if (isLastPage) {
        nextBtn.className = "dis";
        lastBtn.className = "dis";
    } else {
        lastBtn.onclick = function () {
            PagerBtnSearch(pageCount, pageListId);
        };
        nextBtn.onclick = function () {
            PagerBtnSearch(currentPage + 1, pageListId);
        };
    }
    //}

    $(pcLeft).append('共<span class="txt_num">' + totalCount + '</span>条数据<span class="txt-split">|</span>每页显示');
    var selectCon = GetElementByClass("span", "page_select", null);
    var sizeSelect = GetElementByClass("select", "form-control", selectCon);
    var option10 = GetElementByClass("option", "", sizeSelect);
    option10.innerHTML = "10";
    option10.setAttribute("value", 10);

    var option20 = GetElementByClass("option", "", sizeSelect);
    option20.innerHTML = "20";
    option20.setAttribute("value", 20);

    var option50 = GetElementByClass("option", "", sizeSelect);
    option50.innerHTML = "50";
    option50.setAttribute("value", 50);

    $(pcLeft).append(selectCon);
    $(sizeSelect).val(pageSize);
    sizeSelect.onchange = function () {
        var npageSize = parseInt($(this).val());
        if (isNaN(npageSize) || npageSize <= 0) {
            return;
        }
        var newPageCount = Math.ceil(totalCount / npageSize);
        var cnPage = currentPage > newPageCount ? newPageCount : currentPage;
        var newOptions = $.extend(true, {}, searchOptions, { data: { page: cnPage, pageSize: npageSize } });
        PageSearch(newOptions);
    }
    $(searchOptions.pagerEle).append(pagerCon);
    $(window).resize();
}


//创建一个指定Class的Div
function GetDivByClass(className, parentElement) {
    return GetElementByClass("div", className, parentElement);
}

//创建一个指定Id的div
function GetDivById(id, parentElement) {
    return GetElementById("div", id, parentElement);
}

//创建一个指定Class的a标签
function GetLinkByClass(href, className, parentElement) {
    var linkElement = GetElementByClass("a", className, parentElement);
    linkElement.href = href;
    return linkElement;
}

//创建一个指定Id的a标签
function GetLinkById(href, id, parentElement) {
    var linkElement = GetElementById("a", id, parentElement);
    linkElement.href = href;
    return linkElement;
}

//创建一个指定Class的Img标签
function GetImgByClass(src, className, parentElement) {
    var imgElement = GetElementByClass("img", className, parentElement);
    imgElement.src = src;
    return imgElement;
}

//创建一个指定Id的img标签
function GetImgById(src, id, parentElement) {
    var imgElement = GetElementById("img", id, parentElement);
    imgElement.src = src;
    return imgElement;
}

//用指定的类名创建一个指定类型的元素对象
function GetElementByClass(tagName, className, parentElement) {
    if (!tagName) {
        return;
    }
    var elementObject = document.createElement(tagName);
    elementObject.className = className;
    if (parentElement) {
        try {
            parentElement.appendChild(elementObject);
        } catch (e) {

        }
    }
    return elementObject;
}

//使用指定的ID创建一个元素对象
function GetElementById(tagName, id, parentElement) {
    if (!tagName) {
        return;
    }
    var elementObject = document.createElement(tagName);
    elementObject.id = id;
    if (parentElement) {
        try {
            parentElement.appendChild(elementObject);
        } catch (e) {

        }
    }
    return elementObject;
}

//关闭当前弹出页
function CloseCurrentDialogPage() {
    art.dialog.close(true);
}

//获取打开对话框的对象
function DialogOpener() {
    return art.dialog.opener;
}
