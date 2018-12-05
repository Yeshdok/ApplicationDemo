var pageTables = {};

$(function () {
    Layout();
    InitSplit();
    InitContentPanel();
    BindEvent();
});

//绑定页面事件
function BindEvent() {
    $(window).on('resize', function () {
        setTimeout(function () {
            Layout();
            InitContentPanel();
            InitDrawPageDataTable();
        }, 0);
    });
    //复选框事件
    $("body").on('click', '.checkbox-style', function () {
        $(this).toggleClass('checked');
    });

    //高级搜索
    $("body").on("click", ".searchboxbtn", function () {
        OpenSearchBox();
    });
    $("body").on("click", "#hsearch-box #hsb-head .c_btn", function () {
        CloseSearchBox();
    });
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $(window).resize();
    });

    //列表选择框事件
    $("body").on('click', '.cbx_all', function () {
        var checked = $(this).hasClass('checked');
        var parentWrapper = $(this).parents('.DTFC_ScrollWrapper').first();
        parentWrapper.find('.dataTables_scroll .dataTables_scrollBody table tbody tr td .cbx_val').each(function (i, e) {
            if (checked) {
                $(e).addClass("checked");
            } else {
                $(e).removeClass("checked");
            }
        });
        parentWrapper.find('.DTFC_LeftWrapper .DTFC_LeftBodyWrapper .DTFC_LeftBodyLiner table tbody tr td .cbx_val').each(function (i, e) {
            if (checked) {
                $(e).addClass("checked");
            } else {
                $(e).removeClass("checked");
            }
        });
    });
    $("body").on("click", '.DTFC_LeftWrapper .DTFC_LeftBodyWrapper .DTFC_LeftBodyLiner table tbody tr td .cbx_val', function () {
        var parentTable = $(this).parents('table').first();
        var allChecked = parentTable.find('.cbx_val.checked').length >= parentTable.find('.cbx_val').length;
        parentTable.parents('.DTFC_LeftWrapper').first().find('.DTFC_LeftHeadWrapper table thead tr th .cbx_all').each(function (i, e) {
            if (allChecked) {
                $(e).addClass("checked");
            } else {
                $(e).removeClass("checked");
            }
        });
    });
}

//页面布局
function Layout() {
    var winHeight = $(window).height();
    var headHeight = $("#page-head").outerHeight();
    var footHeight = $("#page-foot").outerHeight();
    if (isNaN(headHeight)) {
        headHeight = 0;
    }
    if (isNaN(footHeight)) {
        footHeight = 0;
    }
    $("#page-body").height(winHeight - headHeight - footHeight);
}

//初始化页面分割
function InitSplit() {
    var splitArray = $('.split-vertical-container,.split-horizontal-container');
    splitArray.each(function (i, e) {
        var container = $(e);
        var limit = parseInt(container.attr('data-splitlimit'));
        if (isNaN(limit) || limit <= 0) {
            limit = 50;
        }
        var direction = container.hasClass('split-horizontal-container') ? 'horizontal' : 'vertical';
        container.split({
            orientation: direction,
            limit: limit,
            position: container.attr('data-position'),
            invisible: false,
            onDrag: function () {
                $(window).resize();
            }
        });
    });
}

//面板计算
function InitContentPanel() {
    var contentArray = $('.content-panel');
    contentArray.each(function (i, e) {
        var container = $(e);
        var containerHeight = container.height();
        var headHeight = container.children('.content-panel-head').outerHeight();
        var footHeight = container.children('.content-panel-foot').outerHeight();
        if (isNaN(headHeight)) {
            headHeight = 0;
        }
        if (isNaN(footHeight)) {
            footHeight = 0;
        }
        container.children('.content-panel-body').height(containerHeight - headHeight - footHeight);
    });
}

//初始化数据表
function InitDataTable(options) {
    setTimeout(function () {
        if (!options) {
            return;
        }
        var tableEle = $(options.TableEle);
        if (tableEle.length <= 0) {
            return;
        }
        var tableContentHeight = tableEle.parent().height() - 34;
        var defaultTableOptions = {
            scrollY: tableContentHeight + 'px',
            fixedColumns: {
                rightColumns: 1
            },
            language: {
                infoEmpty: '',
                emptyTable: '暂无数据...'
            },
            searching: false,
            paging: false,
            autoWidth: true,
            dom: 'rtlp',
            ordering: false,
            sScrollX: '100%',
            sScrollXInner: "110%",
            bScrollCollapse: true,
        };
        var initOptions = $.extend(true, defaultTableOptions, options);
        var table = tableEle.DataTable(initOptions);
        pageTables[options.TableEle] = { options: initOptions, table: table };
        if (options.callback) {
            options.callback();
        }
    }, 50);
}

//初始重绘页面表格
function InitDrawPageDataTable() {
    if (!pageTables) {
        return;
    }
    var tableSelectorArray = new Array();
    for (var t in pageTables) {
        tableSelectorArray.push(t);
    }
    InitDrawNowDataTable(tableSelectorArray);
}

//重新初始化当前表格
function InitDrawNowDataTable(tableSelectorArray) {
    if (!tableSelectorArray || tableSelectorArray.length <= 0) {
        return;
    }
    for (var t in tableSelectorArray) {
        InitDrawSingleDataTable(tableSelectorArray[t]);
    }
}

//重新初始化单个数据表格
function InitDrawSingleDataTable(tableSelector, reDraw) {
    if (!tableSelector) {
        return;
    }
    var nowTableItem = pageTables[tableSelector];
    if (!nowTableItem) {
        return;
    }
    var tableWrapper = $(tableSelector + "_wrapper:visible");//user_table_wrapper
    if (tableWrapper.length <= 0) {
        return;
    }
    var containerHeight = tableWrapper.parent().height();
    var wapperInner = tableWrapper.find('.DTFC_ScrollWrapper');
    var tableContentHeight = containerHeight - wapperInner.find('.dataTables_scroll .dataTables_scrollHead').first().height();
    wapperInner.height(containerHeight);
    wapperInner.find('.dataTables_scroll .dataTables_scrollBody').css("height", tableContentHeight + "px");
    if (reDraw) {
        nowTableItem.table.draw();
    }
    var maxHeight = parseInt(wapperInner.find('.dataTables_scroll .dataTables_scrollBody').css("max-height"));
    wapperInner.find('.dataTables_scroll .dataTables_scrollBody').css("max-height", tableContentHeight + "px");
}

//数据表添加新数据
function AddDataTableData(tableSelector, datas) {
    if (!tableSelector || !datas || datas.length <= 0) {
        return;
    }
    var nowTableItem = pageTables[tableSelector];
    if (!nowTableItem) {
        return;
    }
    nowTableItem.table.rows.add(datas).draw();
}

//清除数据表数据
function ClearDataTableData(tableSelector) {
    if (!tableSelector) {
        return;
    }
    var nowTableItem = pageTables[tableSelector];
    if (!nowTableItem) {
        return;
    }
    nowTableItem.table.clear().draw();
    InitDataTableChecked(tableSelector);
}

//替换数据
function ReplaceDataTableData(tableSelector, datas) {
    if (!tableSelector) {
        return;
    }
    var nowTableItem = pageTables[tableSelector];
    if (!nowTableItem) {
        return;
    }
    nowTableItem.table.clear().rows.add(datas).draw();
}

//获取表格选择数据
function GetDataTableCheckedValues(tableSelector) {
    if (!tableSelector) {
        return new Array();
    }
    var dataArray = new Array();
    var tableWrapper = $(tableSelector + "_wrapper");
    if (tableWrapper.length <= 0) {
        return dataArray;
    }
    tableWrapper.find('.DTFC_ScrollWrapper .DTFC_LeftWrapper .DTFC_LeftBodyWrapper .DTFC_LeftBodyLiner table tbody tr td .cbx_val.checked').each(function (i, e) {
        dataArray.push($(e).attr('data-val'));
    });
    return dataArray;
}

//初始化表格选择控件
function InitDataTableChecked(tableSelector) {
    if (!tableSelector) {
        return;
    }
    var tableWrapper = $(tableSelector + "_wrapper");
    var cbxLength = tableWrapper.find('.DTFC_ScrollWrapper .DTFC_LeftWrapper .DTFC_LeftBodyWrapper .DTFC_LeftBodyLiner table tbody tr td .cbx_val').length;
    var checkedLength = tableWrapper.find('.DTFC_ScrollWrapper .DTFC_LeftWrapper .DTFC_LeftBodyWrapper .DTFC_LeftBodyLiner table tbody tr td .cbx_val.checked').length;
    var allChecked = checkedLength >= cbxLength && cbxLength > 0;
    tableWrapper.find('.DTFC_ScrollWrapper .DTFC_LeftWrapper .DTFC_LeftHeadWrapper table thead tr th .cbx_all').each(function (i, e) {
        if (allChecked) {
            $(e).addClass("checked");
        } else {
            $(e).removeClass("checked");
        }
    });
}

//打开高级搜索框
function OpenSearchBox() {
    var boxEle = $("#hsearch-box");
    boxEle.stop();
    boxEle.show();
    //ScrollElement($("#hsb-binner"));
    boxEle.animate({
        "right": "0px"
    }, 500);
};

//关闭高级搜索框
function CloseSearchBox() {
    var boxEle = $("#hsearch-box");
    var boxWidth = boxEle.width();
    boxEle.stop();
    boxEle.animate({
        "right": -boxWidth + "px"
    }, 500, function () {
        boxEle.hide();
    });
};

//绑定回车事件
function BindEnterEvent(func) {
    if (!func) {
        return;
    }
    $(window).keydown(function (e) {
        if (e.keyCode == 13) {
            func();
        }
    });
}

function BindDefaultFormSubmitEnterEvent() {
    BindEnterEvent(function () {
        $("#default-form").submit();
    });
}

//添加初始化表格配置
function AddInitTableConfig(option) {
    if (!option || $.trim(option.table) == '') {
        return;
    }
    $('body').on('click', option.btn, function () {
        var btnEle = $(this);
        $(option.container).addClass('active');
        var init = btnEle.data('table-init');
        if (init) {
            //return;
        }
        InitDrawSingleDataTable(option.table, true);
        btnEle.data('table-init', true);
    });
}
