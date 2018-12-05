$(function(){
	
	Layout();
	$(window).resize(function(){
		Layout();
	});
});

function Layout(){
	var windowHeight=$(window).height();
	var containerHeight=$("#login-container").outerHeight();
	var topValue=0;
	if(windowHeight>containerHeight)
	{
		topValue=(windowHeight-containerHeight)/2-100;
		topValue=topValue<0?0:topValue;
	}
	$("#login-container").css("padding-top",topValue+'px');
}
