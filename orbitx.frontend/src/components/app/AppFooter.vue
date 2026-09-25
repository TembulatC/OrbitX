<template>
  <footer class="orbitx-footer">
    <div class="container footer-wrapper">
      <div class="footer-left">
        <div class="footer-logo-row">
          <div class="footer-logo">OrbitX</div>

          <!-- БЛОК С БЫСТРЫМИ ССЫЛКАМИ (СОЦСЕТИ) -->
          <div class="footer-socials">

            <!-- 1. GitHub -->
            <a href="https://github.com/TembulatC/OrbitX" target="_blank" class="social-link github-link" title="Открыть GitHub проекта">
              <img src="@/assets/logo/github-logo.svg" alt="GitHub" class="social-icon" />
            </a>

            <!-- 2. ТЕЛЕГРАМ -->
            <a href="https://t.me/TKunjut" target="_blank" class="social-link" title="Написать в Telegram">
              <img src="@/assets/logo/telegram-logo.svg" alt="Telegram" class="social-icon" />
            </a>

            <!-- 3. ПОЧТА -->
            <div class="social-link" title="Написать на почту" @click="isEmailOpen = true">
              <img src="@/assets/logo/gmail-logo.svg" alt="Email" class="social-icon" />
            </div>

          </div>
        </div>

        <p class="footer-copyright">
          © 2026 OrbitX Project. Все права защищены.<br />
          Система мониторинга и анализа орбитальных группировок.
        </p>
      </div>

      <div class="footer-right">
        <div class="status-indicator">
          <span class="status-dot"></span>
          <span class="status-text">Сервис работает стабильно</span>
        </div>
        <div class="footer-links">
          <router-link to="/privacy" class="footer-link">Конфиденциальность</router-link>
          <router-link to="/terms" class="footer-link">Условия использования</router-link>
          <!-- По клику открывается окно -->
          <button @click="isSupportOpen = true" class="footer-link-btn">Поддержка</button>
          <router-link to="/donate" class="footer-link footer-link-accent">Поддержать проект</router-link>
        </div>
      </div>
    </div>

    <!-- МОДАЛЬНОЕ ОКНО ПОДДЕРЖКИ -->
    <div v-if="isSupportOpen" class="modal-overlay" @click.self="isSupportOpen = false">
      <div class="modal-box">
        <div class="modal-header">
          <h3 class="modal-title">Техническая поддержка OrbitX</h3>
          <button @click="isSupportOpen = false" class="modal-close-btn">×</button>
        </div>

        <div class="modal-body">
          <div class="support-grid">

            <!-- РАЗДЕЛ 1: ДЛЯ РАЗРАБОТЧИКОВ -->
            <div class="support-section-label">Для разработчиков</div>
            <div class="support-item">
              <span class="support-icon">⚙️</span>
              <div>
                <h4>Технические проблемы и баги</h4>
                <p>Если у вас возникли вопросы по падению сервисов, проксированию и безопасности сайта, разрывам сокетов или ошибкам парсинга OMM-данных — создайте тикет в нашем репозитории.</p>
                <a href="https://github.com/TembulatC/OrbitX/issues" target="_blank" class="support-action">Открыть GitHub Issues →</a>
              </div>
            </div>

            <!-- РАЗДЕЛ 2: ДЛЯ ПОЛЬЗОВАТЕЛЕЙ -->
            <div class="support-section-label">Для пользователей</div>
            <div class="support-item">
              <span class="support-icon">✉️</span>
              <div>
                <h4>Обратная связь и вопросы</h4>
                <p>По любым вопросам использования аналитической платформы, предложениям по сотрудничеству или улучшению интерфейса вы можете написать мне на прямую почту.</p>
                <a class="support-action" @click.prevent="isSupportOpen = false; isEmailOpen = true">tembulats2020@gmail.com</a>
              </div>
            </div>

          </div>
        </div>
      </div>
    </div>

    <!-- МОДАЛЬНОЕ ОКНО ВЫБОРА ПОЧТОВОГО СЕРВИСА -->
    <div v-if="isEmailOpen" class="modal-overlay" @click.self="isEmailOpen = false">
      <div class="modal-box email-modal-box">
        <div class="modal-header">
          <h3 class="modal-title">Написать на почту</h3>
          <button @click="isEmailOpen = false" class="modal-close-btn">×</button>
        </div>

        <div class="modal-body">
          <p class="email-modal-hint">Выберите сервис, в котором хотите открыть письмо на <span class="email-modal-address">{{ EMAIL }}</span></p>

          <div class="email-options">

            <a :href="emailLinks.gmail" target="_blank" rel="noopener" class="email-option" @click="isEmailOpen = false">
              <span class="email-option-icon">
                <img src="@/assets/logo/gmail-logo.svg" alt="Gmail" />
              </span>
              <span class="email-option-name">Gmail</span>
            </a>

            <a :href="emailLinks.outlook" target="_blank" rel="noopener" class="email-option" @click="isEmailOpen = false">
              <span class="email-option-icon">
                <img src="@/assets/logo/outlook-logo.svg" alt="Outlook" />
              </span>
              <span class="email-option-name">Outlook</span>
            </a>

            <a :href="emailLinks.yahoo" target="_blank" rel="noopener" class="email-option" @click="isEmailOpen = false">
              <span class="email-option-icon">
                <img src="@/assets/logo/yahoo-logo.svg" alt="Yahoo Mail" />
              </span>
              <span class="email-option-name">Yahoo Mail</span>
            </a>

            <a :href="emailLinks.mailru" target="_blank" rel="noopener" class="email-option" @click="isEmailOpen = false">
              <span class="email-option-icon">
                <img src="@/assets/logo/mailru-logo.svg" alt="Mail.ru" />
              </span>
              <span class="email-option-name">Mail.ru</span>
            </a>

            <a :href="emailLinks.yandex" target="_blank" rel="noopener" class="email-option" @click="isEmailOpen = false">
              <span class="email-option-icon">
                <img src="@/assets/logo/yandexMail-logo.svg" alt="Yandex.Mail" />
              </span>
              <span class="email-option-name">Yandex.Mail</span>
            </a>

          </div>
        </div>
      </div>
    </div>
  </footer>
</template>


<style scoped>
  .orbitx-footer {
    background-color: #141414; /* Темно-угольный цвет в тон дорожной карты */
    border-top: 1px solid #222222;
    padding: 40px 0;
    width: 100%;
  }

  /* Флекс-контейнер для разделения содержимого на лево/право внутри коробки 1200px */
  .footer-wrapper {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  /* Левый блок */
  .footer-logo {
    font-size: 20px;
    font-weight: 800;
    color: #ffffff;
    margin-bottom: 8px;
    letter-spacing: 0.5px;
  }

  .footer-copyright {
    font-size: 13px;
    line-height: 1.6;
    color: #94a3b8; /* Спокойный серый текст */
  }

  /* Правый блок */
  .footer-right {
    display: flex;
    flex-direction: column;
    align-items: flex-end; /* Прижимаем элементы к правому краю ПК-экрана */
    gap: 16px;
  }

  /* Индикатор статуса систем (зеленая мигающая точка) */
  .status-indicator {
    display: flex;
    align-items: center;
    gap: 8px;
    background: rgba(34, 197, 94, 0.05);
    border: 1px solid rgba(34, 197, 94, 0.15);
    padding: 6px 14px;
    border-radius: 20px;
  }

  .status-dot {
    width: 8px;
    height: 8px;
    background-color: #22c55e;
    border-radius: 50%;
    box-shadow: 0 0 10px #22c55e;
    animation: pulse-dot 2s infinite;
  }

  .status-text {
    font-size: 12px;
    font-weight: 500;
    color: #22c55e;
    margin-bottom: 2px;
  }

  @keyframes pulse-dot {
    0% {
      transform: scale(0.95);
      opacity: 0.5;
    }

    50% {
      transform: scale(1.1);
      opacity: 1;
      box-shadow: 0 0 14px #22c55e;
    }

    100% {
      transform: scale(0.95);
      opacity: 0.5;
    }
  }

  /* Навигационные ссылки в подвале */
  .footer-links {
    display: flex;
    align-items: center;
    flex-wrap: wrap; /* Если ссылки не помещаются в ряд — переносим их ЦЕЛИКОМ, а не по словам */
    row-gap: 10px;
    justify-content: flex-end; /* На ПК/средних экранах держим прижатыми вправо */
  }

  .footer-link {
    font-size: 14px;
    color: #94a3b8;
    text-decoration: none;
    white-space: nowrap; /* Запрещаем тексту ссылки рваться посередине слова */
    transition: color 0.2s ease;
  }

    .footer-link:hover {
      color: #ea75a2;
    }

  /* Кнопка поддержки вместо обычной ссылки */
  .footer-link-btn {
    background: none;
    border: none;
    font-size: 14px;
    color: #94a3b8;
    cursor: pointer;
    padding: 0;
    white-space: nowrap; /* Запрещаем тексту кнопки рваться посередине слова */
    transition: color 0.2s ease;
    font-family: 'Exo 2', sans-serif;
    display: inline-flex;
    align-items: center;
  }

    .footer-link-btn:hover {
      color: #ea75a2;
    }

  /* Добавляем вертикальные разделители между ссылками */
  .footer-links .footer-link:not(:last-child)::after,
  .footer-links .footer-link-btn::after {
    content: '|';
    margin-left: 16px;
    margin-right: 16px;
    color: #2d2d2d;
    font-weight: 300;
    display: inline-block;
    pointer-events: none;
  }

  /* Фон затемнения за окном */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    /* 100dvh вместо 100vh — учитывает реальную видимую высоту на мобильных
       (без него на iOS/Android высота "прыгает" из-за адресной строки браузера) */
    height: 100vh;
    height: 100dvh;
    background-color: rgba(0, 0, 0, 0.6);
    backdrop-filter: blur(5px); /* Размываем сайт на заднем плане */
    display: flex;
    align-items: center;
    justify-content: center;
    /* Отступы по краям + safe-area, чтобы окно не прилипало к краям экрана и не пряталось под чёлку/жестовую полосу */
    padding: 24px calc(20px + env(safe-area-inset-right, 0px)) calc(24px + env(safe-area-inset-bottom, 0px)) calc(20px + env(safe-area-inset-left, 0px));
    z-index: 1000;
  }

  /* Сама коробка модального окна */
  .modal-box {
    max-width: 600px;
    width: 100%;
    /* Ограничиваем высоту видимой областью и разрешаем скролл ВНУТРИ окна,
       если контент (например, список поддержки) не помещается */
    max-height: 100%;
    overflow-y: auto;
    -webkit-overflow-scrolling: touch; /* Плавный инерционный скролл на iOS */
    background-color: #141414;
    border: 1px solid #222222;
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 20px 40px rgba(0, 0, 0, 0.6);
  }

  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
    border-bottom: 1px solid #222222;
    padding-bottom: 12px;
  }

  .modal-title {
    color: #ffffff;
    font-size: 18px;
    font-weight: 700;
  }

  .modal-close-btn {
    background: none;
    border: none;
    color: #64748b;
    font-size: 24px;
    cursor: pointer;
    transition: color 0.2s;
  }

    .modal-close-btn:hover {
      color: #ffffff;
    }

  /* Содержимое поддержки */
  .support-grid {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .support-item {
    display: flex;
    gap: 16px;
    background: rgba(255, 255, 255, 0.02);
    padding: 16px;
    border-radius: 12px;
    border: 1px solid rgba(255, 255, 255, 0.02);
  }

  .support-icon {
    font-size: 24px;
  }

  .support-item h4 {
    color: #ffffff;
    font-size: 15px;
    margin-bottom: 4px;
  }

  .support-item p {
    color: #94a3b8;
    font-size: 13px;
    line-height: 1.5;
    margin-bottom: 8px;
  }

  .support-action {
    color: #ea75a2;
    font-size: 13px;
    font-weight: 600;
    text-decoration: none;
    cursor: pointer;
  }

    .support-action:hover {
      text-decoration: underline;
    }

  .footer-link-accent {
    color: #ea75a2;
    font-weight: 600;
  }

    .footer-link-accent:hover {
      color: #ec4899; /* Становится чуть насыщеннее при наведении */
    }

  /* Метки категорий внутри модального окна */
  .support-section-label {
    font-size: 12px;
    font-weight: 700;
    color: #475569;
    text-transform: uppercase;
    letter-spacing: 1px;
    margin-top: 5px;
    margin-bottom: -5px;
  }

  /* Ряд для логотипа и блока соцсетей */
  .footer-logo-row {
    display: flex;
    align-items: center;
    gap: 16px;
    margin-bottom: 8px;
  }

  /* Контейнер для всех быстрых иконок-ссылок */
  .footer-socials {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 24px;
    gap: 14px;
  }

  /* Ссылка-обертка для иконки */
  .social-link {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    transition: transform 0.2s ease;
    cursor: pointer;
  }

  /* Сама картинка иконки */
  .social-icon {
    width: 22px;
    height: 22px;
    object-fit: contain;
    display: block;
    margin-top: -8px;
  }

  /* Специфичный стиль только для иконки GitHub */
  .github-link .social-icon {
    background-color: #ffffff;
    border-radius: 50%;
    border: 1px solid #141414;
  }

  .social-link:hover {
    transform: translateY(-2px);
  }

  .router-link-active {
    color: #ea75a2;
  }

  .email-modal-box {
    max-width: 420px;
  }

  .email-modal-hint {
    font-size: 13px;
    color: #94a3b8;
    line-height: 1.5;
    margin-bottom: 18px;
  }

  .email-modal-address {
    color: #ea75a2;
    font-weight: 600;
  }

  .email-options {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .email-option {
    display: flex;
    align-items: center;
    gap: 14px;
    padding: 12px 14px;
    border-radius: 10px;
    background: rgba(255, 255, 255, 0.02);
    border: 1px solid rgba(255, 255, 255, 0.02);
    text-decoration: none;
    transition: background-color 0.15s ease, border-color 0.15s ease, transform 0.15s ease;
  }

    .email-option:hover {
      background: rgba(234, 117, 162, 0.06);
      border-color: rgba(234, 117, 162, 0.2);
      transform: translateX(2px);
    }

  .email-option-icon {
    width: 28px;
    height: 28px;
    flex-shrink: 0;
    display: flex;
    align-items: center;
    justify-content: center;
  }

    .email-option-icon img {
      width: 100%;
      height: 100%;
      object-fit: contain;
      display: block;
    }

  .email-option-name {
    font-size: 14px;
    font-weight: 600;
    color: #e2e8f0;
  }

  @media (max-width: 768px) {
    .orbitx-footer {
      padding: 32px 0 calc(24px + env(safe-area-inset-bottom, 0px));
    }

    /* На узких экранах левый и правый блок складываем в колонку */
    .footer-wrapper {
      flex-direction: column;
      align-items: flex-start;
      gap: 24px;
    }

    .footer-right {
      align-items: flex-start; /* На мобильных прижимаем к левому краю, как и левый блок */
      width: 100%;
      gap: 14px;
    }

    /* Ссылки в подвале переносятся на новую строку, а не сжимаются в одну */
    .footer-links {
      flex-wrap: wrap;
      row-gap: 10px;
    }

      .footer-links .footer-link:not(:last-child)::after,
      .footer-links .footer-link-btn::after {
        margin-left: 12px;
        margin-right: 12px;
      }

    /* Модальные окна занимают почти всю ширину экрана с безопасными отступами,
       но остаются по центру и скроллятся внутри, если контента много */
    .modal-overlay {
      padding: 16px;
    }

    .modal-box {
      max-width: 100%;
      padding: 20px;
      border-radius: 16px;
    }

    .support-item {
      flex-direction: column;
      gap: 8px;
    }
  }

  @media (max-width: 480px) {
    .footer-logo-row {
      flex-wrap: wrap;
      row-gap: 10px;
    }

    .status-text {
      font-size: 11px;
    }
  }
</style>

<script setup>
  import { ref } from 'vue'

  const isSupportOpen = ref(false)
  const isEmailOpen = ref(false)

  const EMAIL = 'tembulats2020@gmail.com'

  const emailLinks = {
    gmail: `https://mail.google.com/mail/?view=cm&fs=1&to=${EMAIL}`,
    outlook: `https://outlook.live.com/mail/0/deeplink/compose?to=${EMAIL}`,
    yahoo: `https://compose.mail.yahoo.com/?to=${EMAIL}`,
    mailru: `https://e.mail.ru/compose/?mailto=mailto%3A${EMAIL}`,
    yandex: `https://mail.yandex.ru/compose?mailto=mailto:${EMAIL}`
  }
</script>
