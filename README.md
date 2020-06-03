# URPMultiPass
Test URP multi pass post effect


Unity 2019.3.3f1
URP 7.1.8

Strong recommend to update URP as 7.1.8 have bugs on IOS and Android.

This is a simple test for base idea.
Normally would be creating a renderfeature script with menus that add "EnqueuePass" for each blit. (All my projects are using EnqueuePass, easy to stack post process on up another, change materials or add filters)

**Shaders**

This shaders are not URP! For fast test i didnt wrote them as supossed to be, HLSL.
