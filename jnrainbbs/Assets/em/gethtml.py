# -*- coding: gbk -*-
import os

head='''
<html>
<head>
<title>在超链接中使用事件</title>
<meta http-equiv="content-type" content="text/html;charset=gb2312">
<script src="ms-appx-web:///Assets/jquery-1.11.3.min.js" type="text/javascript" language="javascript"></script>


<script type="text/javascript" language="javascript">
 
$(document).ready(function(){ 
$('img').click( function () { 
var url = $(this).attr("alt"); 
alert(url);
return false; 


}); 
}); 
 
</script>
</head>
<body>
'''

end='''
</body>
</html>
'''

filelist=os.listdir('./')
single="<img src=\"/i/eg_tulip.jpg\"  alt=\"img\" />"
bigsingle=""
content=""
bigcontent=""
for i in filelist:
    seperated=i.split(".")
    seperated=seperated[0]
    if i.find("big")!=-1:
        seperated=seperated.replace("big","")
        seperated="[巨大em"+seperated+"]"
        bigsingle="\n<img src=\"ms-appx-web:///Assets/em/"+i+"\"  alt=\""+seperated+"\" />\n"
        bigcontent=bigcontent+bigsingle
    elif i.find("em")==-1:	
        seperated="[em"+seperated+"]"
        single="\n<img src=\"ms-appx-web:///Assets/em/"+i+"\"  alt=\""+seperated+"\" />\n"
        content=content+single
    '''else:
        seperated="["+seperated+"]"
        single="\n<img src=\"ms-appx-web:///Assets/em/"+i+"\"  alt=\""+seperated+"\" />\n"
        content=content+single'''

content=content+bigcontent
content=head+content
content=content+end
print(content)