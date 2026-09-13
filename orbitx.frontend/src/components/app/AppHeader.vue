<template>
  <header class="orbitx-header">
    <div class="container header-wrapper">
      <a href="/" class="logo-link">
        <img src="@/assets/logo.svg" alt="OrbitX Logo" class="logo-img" />
      </a>

      <nav class="nav-menu">
        <!-- Вместо a href используем router-link to -->
        <router-link to="/" class="nav-link">Главная</router-link>
        <router-link to="/satellites_modeling" class="nav-link">Моделирование</router-link>
      </nav>
    </div>
  </header>
</template>

<style scoped>
  .orbitx-header {
    background-color: #111111; /* Угольно-черный фон шапки */
    height: 80px; /* Фиксированная высота для ПК */
    border-bottom: 1px solid #2d2d2d; /* Тонкая стильная линия снизу */
  }

  .header-wrapper {
    display: flex;
    justify-content: space-between; /* Разносит Лого, Меню и т.д по ширине */
    align-items: center; /* Выравнивает все 3 элемента строго по центру высоты (80px) */
    height: 100%; /* Растягиваем флекс на всю высоту хедера */
  }

  .logo-link {
    display: flex;
    align-items: center; /* Чтобы картинка внутри ссылки центрировалась по вертикали */
    height: 100%; /* Занимает всю высоту флекс-контейнера */
  }

  .logo-img {
    height: 80px; /* Жестко задаем высоту картинки под формат ПК-шапки */
    width: auto; /* Ширина подстроится автоматически, чтобы не искажать пропорции */
    object-fit: contain; /* Защита от деформации картинки */
  }

  .nav-menu {
    display: flex; /* Выстраивает ссылки в один ряд */
    gap: 30px; /* Отступ в 30 пикселей между ссылками на ПК */
  }

  .nav-link {
    color: #ffffff; /* Спокойный белый цвет ссылок в дефолтном состоянии */
    font-size: 16px; /* Комфортный размер шрифта для чтения на ПК */
    font-weight: 500; /* Средняя толщина букв */
    text-decoration: none;
    padding-bottom: 4px; /* Небольшой отступ снизу до будущей черточки */
    border-bottom: 2px solid transparent; /* Создаем НЕВИДИМУЮ прозрачную рамку */
    transition: color 0.3s ease, border-color 0.3s ease; /* Плавно красим и текст, и черточку */
  }

    /* Эффект при наведении курсора мыши на ссылку */
    .nav-link:hover {
      color: #ea75a2; /* Ссылка красиво подсвечивается розовым цветом */
      border-color: #ea75a2;
    }

  .router-link-active {
    color: #ea75a2; /* Страница, на которой сидит юзер, загорится розовым цветом */
    border-color: #ea75a2;
  }
</style>
