# Grid Test (WIP)

**Unity: 2022.3**



**This repo provides:**

* a simple implementation of Grid System.
* a sample to spawn enemies, periodically query using Grid System.
* a test script.

Goto `GridManager.QueryNearby` to explore more.



| Module              | Class               | Description                                                  |
| ------------------- | ------------------- | ------------------------------------------------------------ |
| Core                | Utils               |                                                              |
| Base.Factory        | PooledEnemyFactory  |                                                              |
| Base.PrefabProvider | EnemyPrefabProvider | work with **Spawner**.<br />provide **EnemyPrefab** rule.    |
| Base.Grid           | EnemyIndexer        | update **Instance Index** of Enemy (Grid Manager).<br />update **Instance Position Cache** of Enemy.<br />**Render Gizmos** for debug purpose. |
| Base.Grid           | GridManager         | Add/Remove Indexer → Maintain a instance link.<br />calculate **Grid Key**.<br />can maintain **Instance Position**.<br />use **QueryNearby** to find enemies.<br />**Render Gizmos** for debug purpose. |
| Base.Pool           | EnemyPoolable       | use to mark a specific GO as **Poolable Object**.<br />specific for **Enemy Object**. |
| Base.Pool           | ObjectPool          | A simple **ObjectPool** implementation.                      |
| Base.Pool           | PoolManager         | Use to manage **ObjectPool** instances.                      |
| Base.SpawnBounds    | RectBounds          | Provide a **RectArea (Bounds)** to spawn objects.            |
| Game                | EnemyBatchSpawner   | use to spawn Enemy, with **ObjectPool** support.<br />calculate valid spawn position.<br />can spawn with/without pooling. |
| Temp                | Player              | Use as query center position, periodic query nearby enemies. |
| Temp                | TestGridPerformance |                                                              |



## Guide

### Test Setup

clone repo & open it using Unity 2022.3.

### How to use

Spawn using 
