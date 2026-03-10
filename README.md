# Grid Test (WIP)

**Unity: 2022.3**



**This repo provides:**

* a simple implementation of Grid System.
* a sample.
* a test script.

Goto `GridManager.QueryNearby` to explore more.



| Module              | Description                                                  |
| ------------------- | ------------------------------------------------------------ |
| EnemyIndexer        | update **Instance Index** of Enemy (Grid Manager).<br />update **Instance Position Cache** of Enemy.<br />**Render Gizmos** for debug purpose. |
| EnemyPoolable       | use to mark a specific GO as **Poolable Object**.<br />specific for **Enemy Object**. |
| EnemySpawner        | use to spawn Enemy, with **ObjectPool** support.<br />calculate valid spawn position.<br />can spawn with/without pooling. |
| GridManager         | Add/Remove Indexer → Maintain a instance link.<br />calculate **Grid Key**.<br />can maintain **Instance Position**.<br />use **QueryNearby** to find enemies.<br />**Render Gizmos** for debug purpose. |
| ObjectPool          | A simple **ObjectPool** implementation.                      |
| Player              | Use as query center position, periodic query nearby enemies. |
| TestGridPerformance |                                                              |
| Utils               |                                                              |



## Guide

### Test Setup

clone repo & open it using Unity 2022.3.

### How to use

Spawn using 
