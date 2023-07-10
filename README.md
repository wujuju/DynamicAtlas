静态图集经常会用到，有时候为了极致的性能，降低dc，不得不使用动态图集

动态图集原理也很简单，就是把多张散图，放到一张大图里面

![img](https://picx.zhimg.com/80/v2-89d07d92f1a6016b0e167c3b2ad1792a_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

使用动态图集后

![img](https://picx.zhimg.com/80/v2-a6fe19bd9c92eb33081d187c1d2a2372_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

dc由20变到3

当然使用动态图集也有一些限制

![img](https://picx.zhimg.com/80/v2-b56eff7bf0c3742b041e1c9367041287_720w.png?source=d16d100b)



编辑

添加图片注释，不超过 140 字（可选）

图片必须开启可以读写，这样内存就翻倍了

直接上源码:https://github.com/wujuju/DynamicAtlas.git
