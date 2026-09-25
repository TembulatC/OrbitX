<template>
  <header class="orbitx-header">
    <div class="container header-wrapper">
      <router-link to="/" class="logo-link" @click="isMenuOpen = false">
        <img src="@/assets/logo/logo.svg" alt="OrbitX Logo" class="logo-img" />
      </router-link>

      <!-- Меню для ПК/планшета -->
      <nav class="nav-menu">
        <!-- Вместо a href используем router-link to -->
        <router-link to="/" class="nav-link">Главная</router-link>
        <router-link to="/satellites_modeling" class="nav-link">Моделирование</router-link>
      </nav>

      <!-- Кнопка видна только на мобильных -->
      <button class="burger-btn"
              :class="{ 'is-open': isMenuOpen }"
              @click="isMenuOpen = !isMenuOpen"
              :aria-expanded="isMenuOpen"
              aria-label="Открыть меню">
        <span></span>
        <span></span>
        <span></span>
      </button>
    </div>

    <!-- Выпадающее мобильное меню -->
    <transition name="mobile-nav">
      <nav v-if="isMenuOpen" class="mobile-nav-menu">
        <router-link to="/" class="mobile-nav-link" @click="isMenuOpen = false">Главная</router-link>
        <router-link to="/satellites_modeling" class="mobile-nav-link" @click="isMenuOpen = false">Моделирование</router-link>
      </nav>
    </transition>
  </header>
</template>

<script setup>
  import { ref, watch } from 'vue'
  import { useRoute } from 'vue-router'

  const isMenuOpen = ref(false)
  const route = useRoute()

  // Автоматически закрываем мобильное меню при переходе на другую страницу
  watch(
    () => route.fullPath,
    () => {
      isMenuOpen.value = false
    }
  )
</script>

<style scoped>
  .orbitx-header {
    background-color: #111111; /* Угольно-черный фон шапки */
    border-bottom: 1px solid #2d2d2d; /* Тонкая стильная линия снизу */
    /* Учитываем "чёлку" iPhone, чтобы шапка не заезжала под неё */
    padding-top: env(safe-area-inset-top, 0px);
  }

  .header-wrapper {
    display: flex;
    justify-content: space-between; /* Разносит Лого, Меню и т.д по ширине */
    align-items: center; /* Выравнивает все 3 элемента строго по центру высоты (80px) */
    height: 80px; /* Фиксированная высота для ПК */
  }

  .logo-link {
    display: flex;
    align-items: center; /* Чтобы картинка внутри ссылки центрировалась по вертикали */
    height: 100%; /* Занимает всю высоту флекс-контейнера */
    z-index: 101; /* Логотип всегда поверх мобильного меню */
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

  /* Кнопка скрыта по умолчанию — показываем только на мобильных ниже */
  .burger-btn {
    display: none;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    gap: 5px;
    width: 40px;
    height: 40px;
    z-index: 101;
  }

    .burger-btn span {
      display: block;
      width: 22px;
      height: 2px;
      background-color: #ffffff;
      border-radius: 2px;
      transition: transform 0.25s ease, opacity 0.25s ease;
    }

    /* Превращаем три палочки в крестик, когда меню открыто */
    .burger-btn.is-open span:nth-child(1) {
      transform: translateY(7px) rotate(45deg);
    }

    .burger-btn.is-open span:nth-child(2) {
      opacity: 0;
    }

    .burger-btn.is-open span:nth-child(3) {
      transform: translateY(-7px) rotate(-45deg);
    }

  /* Мобильное выпадающее меню (скрыто на ПК) */
  .mobile-nav-menu {
    display: none;
  }

  /* Анимация появления мобильного меню */
  .mobile-nav-enter-active,
  .mobile-nav-leave-active {
    transition: opacity 0.2s ease, transform 0.2s ease;
  }

  .mobile-nav-enter-from,
  .mobile-nav-leave-to {
    opacity: 0;
    transform: translateY(-8px);
  }

  @media (max-width: 768px) {
    .header-wrapper {
      height: 64px; /* Чуть ниже шапка на мобильных, чтобы не съедать экран */
    }

    .logo-img {
      height: 44px;
    }

    .nav-menu {
      display: none; /* Прячем горизонтальное меню на мобильных */
    }

    .burger-btn {
      display: flex; /* Показываем только на мобильных */
    }

    .mobile-nav-menu {
      display: flex;
      flex-direction: column;
      background-color: #111111;
      border-top: 1px solid #2d2d2d;
      padding: 12px 20px calc(16px + env(safe-area-inset-bottom, 0px));
    }

    .mobile-nav-link {
      color: #ffffff;
      font-size: 16px;
      font-weight: 500;
      text-decoration: none;
      padding: 14px 0;
      border-bottom: 1px solid #2d2d2d;
    }

      .mobile-nav-link:last-child {
        border-bottom: none;
      }

      .mobile-nav-link:hover,
      .mobile-nav-link.router-link-active {
        color: #ea75a2;
      }
  }

  /* Очень маленькие экраны (старые телефоны, узкие устройства) */
  @media (max-width: 360px) {
    .logo-img {
      height: 36px;
    }
  }
</style>
