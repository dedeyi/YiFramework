**YiFramework** 在.net 开发中结合entity framework，借鉴传统三层架构，搭建的一个快速开发框架。

* 底层架构适应于B/S、C/S，同时考虑到缓存、并发、用户回话、异常等处理。
* ORM实体框架对常见CRUD操作进行封装，基于接口的设计更利于灵活实现控制。
* 可支持多个_数据库上下文_同时存在。
* entity framework 的基类Repository暂时只实现基于database first，也能实现code first模式的开发。
* 在B/S系统中,实现了每一个http请求对应的每一个_数据库上下文_只会创建一次，请求结束统一释放上下文资源，保证了每次请求中，上下文也是线程内安全的。
* 在业务逻辑层的增删查改中，封装了对常见字段创建者`CreatorID`、创建时间`CreateTime`、更新时间`UpdateTime`、软删除`IsDel`等的处理
