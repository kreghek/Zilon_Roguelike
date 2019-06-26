### Запуск всех скоростных тестов

-Trait:"integration" -Trait:"benchmark" -Trait:"timeout"  -Project:"Zilon.Bot.Players.DevelopmentTests" -Project:"Zilon.Core.Benchmark"

### Фильтрация по тестам, запускающим бенчи
Trait:"benchmark"

### Запуск бенчей под корку

Бенчи под корку - это консольное приложение. Нужно указать аргументы запуска приложения.

[project-path] - это полный путь к проекту.
[program-files] - это путь к Program Files, где установлена Unity.
[path-to-bench-results] - произвольная папка, куда будут складываться отчёты выполнения бенчей. Результаты так же будут выведены в консольни приложения при запуске.
SchemeCatalog="[project-path]\Zilon.Client\Assets\Resources\Schemes" MonoPath="[program-files]\Unity\Hub\Editor\2018.4.1f1\Editor\Data\MonoBleedingEdge\bin\mono.exe" ArtefactsPath="[path-to-bench-results]" IterationCount=100