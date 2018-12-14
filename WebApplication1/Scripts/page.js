/* 一个简单的分页，每次点击重渲染
******by wuati*****
*/
var ziArray=[];
var ziArray1=[];
var stateObject = {};
var title = "";
var zz=window.location.search;
var lenght;
if(zz){
  var arr01 = zz.split("?");
  var arr02 = arr01[1].split("&");
  for(let x=0;x<arr02.length;x++){
    var arr03 = arr02[x].split("=");
    ziArray[arr03[0]]=arr03[1];
  }
}
else{
  var currentUrl01=window.location.href;
  var newstr=currentUrl01+"?page="+1;
  history.pushState(stateObject,title,newstr);
  var arr00=window.location.href;
  var arr1 = arr00.split("?");
  var arr2 = arr1[1].split("&");
  for(let x=0;x<arr2.length;x++){
    var arr3 = arr2[x].split("=");
    ziArray[arr3[0]]=arr3[1];
  }
  ziArray["page"]="1";
}
(function ($) {
  //默认参数 (放在插件外面，避免每次调用插件都调用一次，节省内存)
  var defaults = {
    //id : '#paging',//id
    leng: 9,//总页数
    activeClass: 'page-active' ,//active类
    firstPage: '首页',//
    lastPage: '末页',
    prv: '«',
    next: '»',
	  clickBack:function(){
	  }
  };
  var opts,myOptions;
  //扩展
  $.fn.extend({
    //插件名称
    page: function (options) {
      //覆盖默认参数
      myOptions = options
      opts = $.extend(defaults, options);
      console.log(opts,998,this)
      //主函数
      return this.each(function () {
        //激活事件
        var obj = $(this);
        var str1 = '';
        var str = '';
        var l = opts.leng;
        lenght=opts.leng;
        if (l > 1&&l < 10) {
          str1 = '<li><a href="javascript:" class="'+ opts.activeClass +'">1</a></li>';
          for (i = 2; i < l + 1; i++) {
            str += '<li><a href="javascript:">' + i + '</a></li>';
          }
        }else if(l > 9){
          str1 = '<li><a href="javascript:" class="'+ opts.activeClass +'">1</a></li>';
          for (i = 2; i < 10; i++) {
            str += '<li><a href="javascript:">' + i + '</a></li>';
          }
          //str += '<li><a href="javascript:">...</a></li>'
        } else {
          str1 = '<li><a href="javascript:" class="'+ opts.activeClass +'">1</a></li>';
        }
        obj.html('<div class="next" style="float:right">' + opts.next + '</div><div class="last" style="float:right">' + opts.lastPage + '</div><ul class="pagingUl">' + str1 + str + '</ul><div class="first" style="float:right">' + opts.firstPage + '</div><div class="prv" style="float:right">' + opts.prv + '</div>');

        obj.on('click', '.next', function () {
          var pageshow = parseInt($('.' + opts.activeClass).html());
          if(pageshow+1<l){
            bianHuan(pageshow+1)
          }
          if(pageshow==l){
            return false;
          }
          if(pageshow == l) {
          }else if(pageshow > l-5&&pageshow < l){
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().next().find('a').addClass(opts.activeClass);
          }else if(pageshow > 0&&pageshow < 6){
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().next().find('a').addClass(opts.activeClass);
          }else {
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().next().find('a').addClass(opts.activeClass);
            fpageShow();
          }
          opts.clickBack(pageshow+1)
          //alert(pageshow+1);
        });
        obj.on('click', '.prv', function () {
          var pageshow = parseInt($('.' + opts.activeClass).html());
          if(pageshow-1>0){
            bianHuan(pageshow-1)
          }
          if (pageshow == 1) {
            return false;
          }else if(pageshow > l-5&&pageshow < l+1){
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().prev().find('a').addClass(opts.activeClass);
                  //this.fpageBranch(pageshow-1);
          }else if(pageshow > 1&&pageshow < 6){
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().prev().find('a').addClass(opts.activeClass);
                  //this.fpageBranch(pageshow-1);
          }else {
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().prev().find('a').addClass(opts.activeClass);
                    //this.fpageBranch(pageshow-1);
            fpageShow();
          }
          opts.clickBack(pageshow-1)
          //alert(pageshow-1);
        });

        obj.on('click', '.first', function(){
          var pageshow = 1;
          bianHuan(pageshow)
          var nowshow = parseInt($('.' + opts.activeClass).html());
          if(nowshow==1){
            return false;
          }
          $('.' + opts.activeClass).removeClass(opts.activeClass).parent().prev().find('a').addClass(opts.activeClass);
          fpagePrv(0);
          opts.clickBack(pageshow)
          //alert(pageshow);
        })
        obj.on('click', '.last', function(){
          var pageshow = l;
          bianHuan(pageshow)
          var nowshow = parseInt($('.' + opts.activeClass).html());
          if(nowshow==l){
            return false;
          }
          if(l>9){
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().prev().find('a').addClass(opts.activeClass);
            fpageNext(8);
          }else{
            $('.' + opts.activeClass).removeClass(opts.activeClass).parent().prev().find('a').addClass(opts.activeClass);
            fpageNext(l-1);
          }
          opts.clickBack(pageshow)
          //alert(pageshow);
        })

        obj.on('click', 'li', function(){
          var $this = $(this);
          var pageshow = parseInt($this.find('a').html());
          bianHuan(pageshow);
          var nowshow = parseInt($('.' + opts.activeClass).html());
         // console.log(l,256)
          if(pageshow==nowshow){
            return false;
          }
          if(l>9){
            console.log(1234567,pageshow,l)
            if(pageshow > l-5&&pageshow < l+1){
              $('.' + opts.activeClass).removeClass(opts.activeClass);
              $this.find('a').addClass(opts.activeClass);
              fpageNext(8-(l-pageshow));
            }else if(pageshow > 0&&pageshow < 5){
              $('.' + opts.activeClass).removeClass(opts.activeClass);
              $this.find('a').addClass(opts.activeClass);
              fpagePrv(pageshow-1);
            }else{
              $('.' + opts.activeClass).removeClass(opts.activeClass);
              $this.find('a').addClass(opts.activeClass);
              fpageShow();
            }
          }else{
            console.log(123456)
            $('.' + opts.activeClass).removeClass(opts.activeClass);
            $this.find('a').addClass(opts.activeClass);
          }
          opts.clickBack(pageshow)
        })
        bian();
        function fpageShow(){
          var pageshow = parseInt($('.' + opts.activeClass).html());
          var pageStart = pageshow - 4;
          var pageEnd = pageshow + 5;
          var str1 = '';
          for(i=0;i<9;i++){
            str1 += '<li><a href="javascript:" class="">' + (pageStart+i) + '</a></li>'
          }
          obj.find('ul').html(str1);
          obj.find('ul li').eq(4).find('a').addClass(opts.activeClass);
		      
        }

        function fpagePrv(prv){
          var str1 = '';
          if(l>8){
            for(i=0;i<9;i++){
              str1 += '<li><a href="javascript:" class="">' + (i+1) + '</a></li>'
            }
          }else{
            for(i=0;i<l;i++){
              str1 += '<li><a href="javascript:" class="">' + (i+1) + '</a></li>'
            }
          }
          obj.find('ul').html(str1);
          obj.find('ul li').eq(prv).find('a').addClass(opts.activeClass);
        }

        function fpageNext(next){
          var str1 = '';
          if(l>8){
            for(i=l-8;i<l+1;i++){
              str1 += '<li><a href="javascript:" class="">' + i + '</a></li>'
            }
           obj.find('ul').html(str1);
           obj.find('ul li').eq(next).find('a').addClass(opts.activeClass);
          }else{
            for(i=0;i<l;i++){
              str1 += '<li><a href="javascript:" class="">' + (i+1) + '</a></li>'
            }
           obj.find('ul').html(str1);
           obj.find('ul li').eq(next).find('a').addClass(opts.activeClass);
          }
        }
        
        //刷新不变
        function bian() {
          $('.pagingUl li').find('a').removeClass(opts.activeClass);
          for(let x=0;x<$('.pagingUl li').length;x++){
            if(parseInt($('.pagingUl li').eq(x).find('a').html())==parseInt(ziArray['page'])){
              if($('.pagingUl li').eq(x)){
                $('.pagingUl li').eq(x).find('a').addClass(opts.activeClass);
              }
            }
            else{
              var pageshow = parseInt(ziArray['page']);
              var bianhuanPan=pageshow + 5-lenght;
              var bainahunPan0=pageshow - 4;
              if(bianhuanPan>=5){
                var pageStart = pageshow - 8;
                var pageEnd = pageshow + 0;
              }
              if(bianhuanPan==4){
                var pageStart = pageshow - 7;
                var pageEnd = pageshow + 1;
              }
              if(bianhuanPan==3){
                var pageStart = pageshow - 6;
                var pageEnd = pageshow + 2;
              }
              if(bianhuanPan==2){
                var pageStart = pageshow - 5;
                var pageEnd = pageshow + 3;
              }
              if(bianhuanPan==1){
                var pageStart = pageshow - 4;
                var pageEnd = pageshow + 4;
              }
              if(bianhuanPan<=0){
                var pageStart = pageshow - 4;
                var pageEnd = pageshow + 5;
                if(bainahunPan0==0){
                  var pageStart = pageshow - 3;
                  var pageEnd = pageshow + 6;
                }
                if(bainahunPan0==-1){
                  var pageStart = pageshow - 2;
                  var pageEnd = pageshow + 7;
                }
                if(bainahunPan0==-2){
                  var pageStart = pageshow - 1;
                  var pageEnd = pageshow + 8;
                }
                if(bainahunPan0==-3){
                  var pageStart = pageshow - 0;
                  var pageEnd = pageshow + 9;
                }
              }
              var str1 = '';
              for(i=9;i<0;i--){
                str1 += '<li><a href="javascript:" class="">' + (pageEnd-i) + '</a></li>'
              }
              for(i=0;i<9;i++){
                  str1 += '<li><a href="javascript:" class="">' + (pageStart+i) + '</a></li>'
              }
              obj.find('ul').html(str1);
              for(let x=0;x<$('.pagingUl li').length;x++){
                if(parseInt($('.pagingUl li').eq(x).find('a').html())==pageshow){
                  if($('.pagingUl li').eq(x)){
                    $('.pagingUl li').eq(x).find('a').addClass(opts.activeClass);
                  }
                }
              }
            }
          }
        }
        //返回变化
        window.addEventListener('popstate', function (){
          var arr00=window.location.href;
          var arr1 = arr00.split("?");
          var arr2 = arr1[1].split("&");
          for(let x=0;x<arr2.length;x++){
            var arr3 = arr2[x].split("=");
            ziArray1[arr3[0]]=arr3[1];
          }
          ziArray["page"]=ziArray1["page"];
          bian();
        });
      });
    },
    setLength: function(newLength){
      myOptions.leng = newLength
      $(this).html('')
      $(this).unbind()
      $(this).page(myOptions)
    }
  })
})(jQuery);
function bianHuan(pageshow){
  var currentUrl01=window.location.href;
  var arr01 = currentUrl01.split("?");
  ziArray["page"]=""+pageshow+"";
  var zi="";
  for(var xi in ziArray){
    zi+=xi+"="+ziArray[xi]+"&";
  }
  if(zi.substr(zi.length-1,zi.length)=="&"){
    zi=zi.substr(0,zi.length-1)
  }
  var newstr=arr01[0]+"?"+zi;
  history.pushState(stateObject,title,newstr);
  console.log($('.pagingUl li').length);
  console.log(parseInt(ziArray['page']));
}
