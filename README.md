# Avril

# 计划制作内容
## Avril(玩家)功能
### 攻击
* 魔法攻击
* 物理攻击
### 魔法辅助
* 投掷法杖，并可以瞬移会发法杖处；如果超过一定距离，则召回法杖
* 障壁生成（禁止移动等一系列操作，持续减少魔力，有一定的耐久值）
* 回复术，可以与障壁一起叠加（魔法混合系统），在障壁破裂的瞬间回复一定的血量
* 元素互动，即补魔
### 翻滚
有无地帧，但是后摇明显，谨慎使用
### 跳跃
* 普通跳跃
* 可以进行下坠攻击（待定）
* 附魔之后，在下落时可以进行下坠魔法攻击

## 魔力系统，魔力混合系统 (待完善)

 
# 日志
## 2020/12/31
通过 2D Animation 和 2D IK 完成Avril的 Idle、Walk、Run、Jump 动画</br>
绘制Avril的动画机(Idle、Walk、Run、Jump)</br>
实现Avril的移动、跳跃功能</br>

## 2020/01/02
对Avril的绘制了法杖，并对一些细节进行了修改</br>
绘制了Magic动画</br>

## 2020/01/03
寻找魔法相关的AFX</br>
添加了奥术飞弹（暂定）的预制体</br>
完成了释放奥数飞弹的操作</br>
完善原有代码</br>

## 2021/01/06
制作了投掷物品的动作</br>
实现了投掷法杖并瞬移到法杖处的功能</br>
制作了翻滚动作</br>
实现了翻滚操作</br>
修改了Avril的控制逻辑，添加了一些状态量进行状态的控制</br>

## 2021/01/07
制作了物理攻击动画</br>
完成了物理攻击逻辑</br>

## 2021/01/08
制作了瞬移至法杖的动作，并完善了相关的逻辑
实现了魔法飞弹的储存

## 2021/01/09
整理代码，将魔法系统另起一个类进行管理</br>
设计魔力融合的雏形：</br>
* 融合：元素融合、同系融合
* 附魔：
* 1. 释放魔法的过程中，附加上魔法
* 2. 武器附魔

## 2021/01/10
整理代码，将魔力系统改为有限状态机实现（FSM）

## 2021/01/11
修改Missile、MagicSystem 和 MagicMissileState</br>
### 添加了 missile(飞弹)向 storage 状态切换的过渡状态(从有到无，从无到有)。</br>
实现方式为：通过协程实现延迟操作。(协程嵌套)</br>
### 实现了 missile 在 storage 状态下的范围随机移动
构思如何实现魔力混合系统</br>

## 2021/01/13
### 魔力混合系统进度汇总
完成了攻击系魔法的不同种之间的融合

## 2021/01/14
更改牵引系魔法的效果：随时间逐渐消失，牵引力同(finished)</br>
追踪系魔法添加运动曲线，实现类似LookAt的功能(finished)</br>
实现魔法混合的动态效果(finished)</br>

已知bug：</br>
1、在储存过程中进行魔法混合(finished)</br>
修正方案：数据在彻底进入存储状态是进行存储，而并未一开始就存储</br>
2、快速点击魔法混合之后，出现数据混乱(finished)</br>
修正方案：提前修改数据信息即可</br>

3、未混合完毕的魔法被释放的处理</br>
处理方案：直接将混合好的魔法释放</br>

修复了跑步过程中，由于相对速度的问题造成的魔法无法混合的问题。</br>

更改牵引系魔法的效果：缓慢移动，触碰到敌人后迅速扩张，产生吸力(整个过程无伤害)(finished)</br>

## 2021/01/15
修复了导出后，释放魔法时移动，导致魔法消失的问题。(中断魔法判断出现问题)</br>
实现魔法击中的效果(finished)</br>
初步完成辅助魔法的实现(辅助系魔法和攻击系魔法分成两个槽位)</br>

# 计划
设计辅助系魔法的功能和相对应的类</br>

功能：</br>
1、辅助魔法和攻击魔法之间不能进行混合</br>
2、辅助魔法混合是效果上的叠加，即不会产生新的魔法</br>
3、辅助魔法分为治愈魔法和护盾魔法，使用两种不同的动作</br>
4、释放辅助魔法时不能进行移动

整理、完善魔法系统。</br>

设计Avril的属性</br>

设计Avril操作相关的简单UI</br>

设计Boss的行为，通过有限状态机来实现</br>

# Fight!!!
