# 简述
Avalonia 学习项目（[课程链接](https://www.bilibili.com/video/BV12rHrekEZs?spm_id_from=333.788.videopod.sections&vd_source=a13951d1d0a20d95b33876b57ca0bcde)），尝试 AOT 发布。

# 特点
- 使用 Jab 库进行单例模式、依赖注入；
- 因为 AOT 用不了 Avalonia.Xaml.Behaviors 库，事件只好直接绑定；
- 用 JsonSerializerContext 源生成的方式反序列化。

## 离经叛道
- 测试放在同一个项目，模仿一下 Rust；
- 变量、常量都用蛇形命名法。因为小驼峰视觉效果就是垃圾，尾大不掉，干脆全小写；而常量比变量安全，又不是危险的宏，那也小写算了；
- 凡是超过 10 个字母的单词必要缩写，去除元音；8 个以下一般不缩写；8～10 看心情。


# 已知问题
在 Windows 上能 debug 运行，但 AOT 报错找不到 linker，装了 VS C#、C++ 环境还是这样，懒得折腾了。在 Linux（NixOS 25.05） 上能正常 AOT。

