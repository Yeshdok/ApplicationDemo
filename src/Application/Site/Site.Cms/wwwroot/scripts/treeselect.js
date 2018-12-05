var CTreeSelect=function(options){
	this.defaultOptions={
		containerEle:'.ctreeselect-container',//容器元素
		treeConfig:{
		},
		multiSelect:false,
		showFullText:true,
		selectedCallback:null,
		width:200,
		height:300,
		selectCallback:null,
		placetext:'请选择数据',
		inputText:'',
		selectedValue:null
	};
	
	this.settings=$.extend({},this.defaultOptions,options);
	var thisObj=this;
	this.Container=null;
	this.TreeObject=null;
	this.TreeEle=null;
	
	//初始化
	this.Init=function(){
		var container=this.Container=$(this.settings.containerEle);
		if(container.length<=0){
			return;
		}
		this.settings.treeConfig.treeSettings.check={
			enable:this.settings.multiSelect
		};
		if(!this.settings.multiSelect){
			if(this.settings.treeConfig.treeSettings.callback){
				this.settings.treeConfig.treeSettings.callback.onClick=thisObj.TreeNodeClick;
			}
			else{
				this.settings.treeConfig.treeSettings.callback={
					onClick:thisObj.TreeNodeClick
				}
			}
		}else{
			if(this.settings.treeConfig.treeSettings.callback){
				this.settings.treeConfig.treeSettings.callback.onCheck=thisObj.TreeNodeCheck;
			}
			else{
				this.settings.treeConfig.treeSettings.callback={
					onCheck:thisObj.TreeNodeCheck
				}
			}
			this.settings.treeConfig.treeSettings.check.chkboxType={'Y':'','N':''}
		}
		
		container.addClass('ztree-container');
		container.css('position','relative');
		container.css('z-index','999999');
		//创建下拉框
		var txtContainer=$('<div class="ctreeselect-text-container"><i class="ctreeselect-text-ico micon micon-down"></i><input type="text" class="form-control ctreeselect-text-input" placeholder="'+this.settings.placetext+'" readonly="readonly" value="'+this.settings.inputText+'"/></div>')
		txtContainer.width(this.settings.width);
		txtContainer.css('position','absolute').css("left",'0px').css('top','0px');
		container.append(txtContainer);
		txtContainer.find('.ctreeselect-text-input').outerWidth(txtContainer.width()-txtContainer.find('.ctreeselect-text-ico').width()).outerHeight(txtContainer.height());
		txtContainer.css('line-height',txtContainer.height()+'px');
		
		var treeContainer=$('<div class="ctreeselect-tree-container"></div>')
		treeContainer.width(this.settings.width);
		treeContainer.css('position','absolute').css("left",'0px').css('top',txtContainer.outerHeight()-1);
		container.append(treeContainer);
		
		var treeList=$('<div class="ctreeselect-data-container"></div>');
		treeList.css('max-height',this.settings.height);
		var id='treedata_'+this.settings.containerEle.substring(1);
		var treeData=$('<ul class="ctreeselect-date-list ztree" id="'+id+'"></ul>');
		treeContainer.append(treeList);
		treeList.append(treeData);
		this.TreeEle=treeData;
		this.InitTree();
		
		txtContainer.find('.ctreeselect-text-ico').click(function(){
			thisObj.ToggleShow();
		});
		
		if(this.settings.multiSelect){
			var footTool=$('<div class="ctreeselect-foot"><button type="button" class="ctreeselect-selectallbtn btn btn-sm btn-primary">全选</button> <button type="button" class="ctreeselect-cacheallbtn btn btn-sm btn-default">清空</button> <button type="button" class="ctreeselect-cachebtn btn btn-sm btn-danger">关闭</button></div>');
			treeContainer.append(footTool);
			footTool.find('.ctreeselect-cachebtn').click(function(){
				thisObj.ToggleShow();
			});
			footTool.find('.ctreeselect-selectallbtn').click(function(){
				thisObj.TreeObject.checkAllNodes(true);
				thisObj.TreeNodeCheck();
			});
			footTool.find('.ctreeselect-cacheallbtn').click(function(){
				thisObj.TreeObject.checkAllNodes(false);
				thisObj.TreeNodeCheck();
			});
		}
	}
	
	
	this.ToggleShow=function(){
		thisObj.Container.toggleClass('open');
		if(thisObj.Container.hasClass('open')){
			thisObj.Container.find('.ctreeselect-text-ico').addClass('micon-up').removeClass('micon-down');
		}else{
			thisObj.Container.find('.ctreeselect-text-ico').removeClass('micon-up').addClass('micon-down');
		}
	}
	
	this.TreeNodeCheck=function(event, treeId, treeNode){
		var names=thisObj.GetCheckedNames();
		thisObj.Container.find('.ctreeselect-text-input').val(names.join(','));
		var nodes=thisObj.TreeObject.getCheckedNodes();
		if(thisObj.settings.selectCallback){
			thisObj.settings.selectCallback(nodes);
		}
	}
	
	this.TreeNodeClick=function(event, treeId, treeNode){
		if(thisObj.settings.multiSelect){
			return;
		}
		thisObj.SelectNode(treeNode);
		thisObj.ToggleShow();
	}
	
	this.SelectNode=function(treeNode){
		var txt=thisObj.JoinParentNames(treeNode);
		thisObj.Container.find('.ctreeselect-text-input').val(txt);
		if(thisObj.settings.selectCallback){
			thisObj.settings.selectCallback([treeNode]);
		}
	}
	
	this.GetCheckedNames=function(){
		var checkedNodes=thisObj.TreeObject.getCheckedNodes();
		var names=new Array();
		for(var n in checkedNodes){
			var node=checkedNodes[n];
			names.push(node.name);
		}
		return names;
	}
	
	this.JoinParentNames=function(treeNode){
		var parentNodes = new Array();
        var nowNode = treeNode;
        var parentNodeNames = '';
//      while (true) {
//          var parentNode = nowNode.getParentNode();
//          if (!parentNode) {
//              break;
//          }
//          nowNode = parentNode;
//          parentNodeNames = parentNode.name + '->' + parentNodeNames;
//      }
        parentNodeNames += treeNode.name;
        return parentNodeNames;
	}
	
	this.InitSelectedValue=function(){
		if(!thisObj.settings.selectedValue||thisObj.settings.selectedValue.length<=0){
			return;
		}
		if(thisObj.settings.multiSelect){
			
		}else{
			var firVal=thisObj.settings.selectedValue[0];
			var node=thisObj.TreeObject.getNodeByParam("id",firVal);
			if(node){
				thisObj.SelectNode(node);
			}
		}
	}
	
	this.InitTree=function(){
		this.TreeObject=$.fn.zTree.init(this.TreeEle, this.settings.treeConfig.treeSettings,this.settings.treeConfig.nodes);
	}
	
	//隐藏大于或等于指定等级的节点
	this.HideNodesByLevel=function(startLevel){
		thisObj.InitTree();
		var nodes=thisObj.TreeObject.getNodesByFilter(function (node) {
            return node.level >= startLevel;
        }, false);
      	thisObj.TreeObject.hideNodes(nodes);
	}
	
	this.Init();
	this.InitSelectedValue();
}
