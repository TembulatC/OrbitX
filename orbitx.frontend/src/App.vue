<template>
  <div class="app-layout">
    <!-- Вызываем шапку -->
    <AppHeader />

    <!-- Основная область для контента -->
    <main class="main-content">
      <div class="container">
        <router-view /> <!-- Сюда роутер сам будет вставлять наш контент -->
      </div>
    </main>

    <!-- Вызываем подвал -->
    <AppFooter />

    <button class="scroll-top-btn" v-show="isVisible" @click="scrollToTop" title="Наверх">↑</button>
  </div>
</template>

<script setup>
  import { ref, onMounted, onUnmounted } from 'vue'
  import AppHeader from './components/app/AppHeader.vue'
  import AppFooter from './components/app/AppFooter.vue'

  // Переменная, отвечающая за видимость кнопки
  const isVisible = ref(false)

  // Функция плавной прокрутки на самый верх
  const scrollToTop = () => {
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    })
  }

  // Функция, которая проверяет, где сейчас находится скролл
  const handleScroll = () => {
    isVisible.value = window.scrollY > 300
  }

  // Включаем слежку при загрузке сайта и выключаем при уходе, чтобы не перегружать память ПК
  onMounted(() => {
    window.addEventListener('scroll', handleScroll)
  })

  onUnmounted(() => {
    window.removeEventListener('scroll', handleScroll)
  })
</script>

<style>
  .app-layout {
    display: flex;
    flex-direction: column; /* Выстраивает элементы вертикально: Header -> Main -> Footer */
    min-height: 100%; /* Растягивается минимум на всю высоту экрана */
  }

  .main-content {
    flex-grow: 1;
  }

  .container {
    max-width: 1200px; /* На больших мониторах контент не станет шире 1200px */
    width: 100%; /* Если окно ПК станет меньше 1200px (например, 1000px),контейнер сожмется вместе с ним */
    margin: 0 auto; /* Центрирование */
    padding: 0 20px; /* Безопасные отступы, чтобы при сильном сжатии элементы не прилипали к краям */
  }

  .scroll-top-btn {
    position: fixed;
    bottom: 40px;
    left: 40px; /* Размещаем строго слева */
    width: 45px;
    height: 45px;
    border-radius: 50%;
    background-color: #141414; /* Темный фон в цвет блоков */
    border: 1px solid #2d2d2d;
    color: #94a3b8;
    font-size: 20px;
    font-weight: bold;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 99; /* Чтобы кнопка всегда была поверх картинок и радара */
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
    transition: all 0.3s ease;
  }

    /* Эффект при наведении мышки */
    .scroll-top-btn:hover {
      color: #ffffff;
      background-color: #ea75a2;
      border-color: #ea75a2;
      box-shadow: 0 0 15px rgba(236, 72, 153, 0.4);
      transform: translateY(-3px); /* Кнопка слегка приподнимается */
    }

  @media (max-width: 768px) {
    .container {
      padding: 0 16px; /* Чуть уже отступы, чтобы не "съедать" контент на узких экранах */
    }

    .scroll-top-btn {
      width: 40px;
      height: 40px;
      font-size: 18px;
      bottom: calc(20px + env(safe-area-inset-bottom, 0px));
      left: 20px;
    }
  }

  @media (max-width: 480px) {
    .container {
      padding: 0 12px;
    }
  }

  /* Наведение мышью не работает на тач-устройствах — не даём кнопке залипать в розовом */
  @media (hover: none) {
    .scroll-top-btn:hover {
      background-color: #141414;
      border-color: #2d2d2d;
      color: #94a3b8;
      transform: none;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
    }
  }
</style>
