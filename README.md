# Консольный веб-сервис Зоомагазин
Среда разработки: VS 2013

Простое конольное приложение, принимающее запросы POST и GET.
- POST создает питомца (два параметра Name и Type), и возвращает его в формате JSON:
  http://localhost:4141/pets?name=Meow&type=Cat

- GET возвращает текущий набор питомцев в формате JSON:
  http://localhost:4141/pets

Репозиторий питомцев в виде переменной, без использования хранилища.
