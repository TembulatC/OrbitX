<template>
  <section class="hero-section">
    <div class="container hero-wrapper">

      <!-- Текстовый блок слева -->
      <div class="hero-content">
        <h1 class="hero-title">
          Интеллектуальный мониторинг <span class="accent-text">орбитальных группировок</span>
        </h1>
        <p class="hero-subtitle">
          Платформа OrbitX предоставляет продвинутые инструменты SGP4-моделирования для современных спутниковых систем в реальном времени. Модули телеметрии и геомагнитного трекинга находятся в разработке...
        </p>

        <!-- Кнопки действий -->
        <div class="hero-actions">
          <router-link to="/satellites_modeling" class="btn btn-primary">
            Открыть каталог
          </router-link>
          <button @click="scrollToRoadmap" class="btn btn-secondary">
            Узнать больше
          </button>
        </div>
      </div>

      <!-- Декоративный элемент справа (например, заглушка под графику/радар) -->
      <div class="hero-graphics">
        <div class="radar-placeholder">
          <div class="radar-line"></div>
        </div>
      </div>

    </div>
  </section>

  <section id="roadmap" class="roadmap-section">
    <div class="container">
      <div class="roadmap-box">
        <!-- Заголовок секции -->
        <h2 class="section-title">Дорожная карта <span class="accent-text">развития OrbitX</span></h2>
        <p class="section-subtitle">Этапы интеграции модулей и развертывания аналитической платформы</p>

        <!-- Контейнер для шагов карты -->
        <div class="roadmap-timeline">

          <!-- Шаг 1 (Выполнено) -->
          <div class="roadmap-item completed">
            <div class="roadmap-badge">✓</div>
            <div class="roadmap-content">
              <span class="roadmap-date">Этап 1 — Интегрировано</span>
              <h3 class="roadmap-item-title">SGP4-Моделирование</h3>
              <div class="roadmap-item-text">
                <p style="margin-bottom: 8px; font-weight: 600; color: #ffffff;">
                  Высокоточное прогнозирование и визуализация орбитальных траекторий:
                </p>
                <ul>
                  <li>Расчет координат космических аппаратов на основе актуальных OMM-данных.</li>
                  <li>Интерактивное отображение местонахождения спутников в реальном времени.</li>
                  <li>Поддержка масштабных орбитальных группировок без потери производительности.</li>
                </ul>
              </div>
            </div>
          </div>

          <!-- Шаг 2 (В разработке) -->
          <div class="roadmap-item completed">
            <div class="roadmap-badge">2</div>
            <div class="roadmap-content">
              <span class="roadmap-date">Этап 2 — В разработке</span>
              <h3 class="roadmap-item-title">Модуль: Телеметрия</h3>
              <p class="roadmap-item-text">
                Интерактивные дашборды для контроля внутренних систем аппарата. Анализ энергопотребления (EPS), температурных режимов (TCS), статуса радиосвязи (COMMS) и ориентации в пространстве (ADCS) по логам.
              </p>
            </div>
          </div>

          <!-- Шаг 3 (Будущее) -->
          <div class="roadmap-item">
            <div class="roadmap-badge">3</div>
            <div class="roadmap-content">
              <span class="roadmap-date">Этап 3 — В проекте</span>
              <h3 class="roadmap-item-title">Модуль: Геомагнитный трекинг</h3>
              <p class="roadmap-item-text">
                Мониторинг космической погоды и солнечной активности. Анализ Kp-индекса и прогнозирование аномалий (включая SAA) для предотвращения деградации орбит (Atmospheric Drag) и сбоев бортовой электроники.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
  /* Внешняя секция на всю ширину экрана с фоном */
  .hero-section {
    display: flex;
    align-items: center;
    min-height: calc(100vh - 80px);
    /* Добавляем внутренние безопасные отступы сверху и снизу (по 40px), */
    /* чтобы при сильном сжатии окон текст не упирался в шапку и дорожную карту */
    padding: 40px 0;
    margin: 0;
    background-color: #1a1a1a;
  }

  /* Флекс-контейнер для разделения на лево/право */
  .hero-wrapper {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 40px;
  }

  /* Левая текстовая часть */
  .hero-content {
    max-width: 600px; /* Чтобы текст не растягивался слишком широко */
  }

  .hero-title {
    font-size: clamp(28px, 5.5vw, 48px);
    font-weight: 800;
    line-height: 1.2;
    margin-bottom: 24px;
    color: #ffffff;
  }

  /* Выделение цветом важных космических терминов */
  .accent-text {
    color: #ea75a2;
  }

  .hero-subtitle {
    font-size: clamp(15px, 2.2vw, 18px);
    line-height: 1.6;
    color: #94a3b8; /* Спокойный серо-голубой цвет текста */
    margin-bottom: 40px;
  }

  /* Блок с кнопками */
  .hero-actions {
    display: flex;
    gap: 20px;
  }

  /* Общие стили для кнопок */
  .btn {
    display: inline-flex;
    align-items: center;
    padding: 14px 28px;
    text-decoration: none;
    font-size: 16px;
    font-weight: 600;
    border-radius: 8px;
    transition: all 0.2s ease;
    cursor: pointer;
  }

  .btn-primary {
    background-color: #ea75a2;
    color: #ffffff;
  }

    .btn-primary:hover {
      background-color: #c74275;
      transform: translateY(-2px); /* Эффект легкого всплытия при наведении */
    }

  .btn-secondary {
    background-color: transparent;
    color: #ffffff;
    border: 1px solid #475569;
    font-family: "Exo 2", sans-serif;
  }

    .btn-secondary:hover {
      background-color: #334155;
      border-color: #64748b;
    }

  /* Правая декоративная часть */
  .hero-graphics {
    flex-grow: 1;
    display: flex;
    justify-content: center;
  }

  /* Простая имитация космического радара с помощью CSS */
  .radar-placeholder {
    width: 320px;
    height: 320px;
    border: 2px dashed #ea75a2;
    border-radius: 50%;
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    background: radial-gradient(circle at 50% 50%, rgba(236, 72, 153, 0.1) 0%, transparent 80%);
    box-shadow: 0 0 120px rgba(236, 72, 153, 0.08);
  }

    .radar-placeholder::before {
      content: '';
      width: 150px;
      height: 150px;
      border: 1px solid #ea75a2;
      border-radius: 50%;
      opacity: 0.5;
    }

  .radar-line {
    position: absolute;
    width: 50%;
    height: 2px;
    background: linear-gradient(90deg, transparent, #ea75a2);
    top: 50%;
    left: 50%;
    transform-origin: left center;
    animation: radar-rotate 4s linear infinite; /* Заставим линию крутиться */
  }

  @keyframes radar-rotate {
    from {
      transform: rotate(0deg);
    }

    to {
      transform: rotate(360deg);
    }
  }

  .roadmap-section {
    min-height: 100vh;
    padding: 0;
    margin: 0;
    margin-bottom: 80px;
    background-color: #1a1a1a;
    /* Включаем Flexbox, чтобы большая темная коробка встала по центру экрана */
    display: flex;
    align-items: center; /* Центрирование по вертикали */
    justify-content: center; /* Центрирование по горизонтали */
  }

  /* Наш большой темный прямоугольник-остров */
  .roadmap-box {
    max-width: 1000px;
    width: 100%;
    padding: 50px 40px; /* Симметричные, аккуратные отступы сверху и снизу */
    background-color: #141414; /* Угольно-темный фон коробки */
    border: 1px solid #222222;
    border-radius: 24px;
  }

  /* Заголовки внутри темного блока */
  .section-title {
    font-size: 40px;
    font-weight: 800;
    text-align: center;
    margin-bottom: 12px;
    color: #ffffff;
    letter-spacing: -0.5px;
  }

  .section-subtitle {
    font-size: 16px;
    color: #64748b;
    text-align: center;
    margin-bottom: 45px; /* Уплотнили расстояние до карточек, чтобы убрать пустоту */
  }

  /* Общая линия таймлайна */
  .roadmap-timeline {
    position: relative;
    max-width: 720px; /* Сжали ширину карточек до 720px, чтобы они смотрелись аккуратно */
    margin: 0 auto;
    padding-left: 50px;
  }

    .roadmap-timeline::before {
      content: '';
      position: absolute;
      top: 15px;
      bottom: 15px;
      left: 15px;
      width: 2px;
      /* Градиент переливается из ярко-розового в темно-розовый и мягко гаснет в цвет подложки (#141414) */
      background: linear-gradient(to bottom, #ea75a2 0%, #ec4899 50%, #141414 100%);
      opacity: 0.8;
    }

  /* Блок одного шага */
  .roadmap-item {
    position: relative;
    margin-bottom: 40px; /* Аккуратный шаг между блоками */
  }

    .roadmap-item:last-child {
      margin-bottom: 0; /* Полностью убирает лишний отступ снизу у последней карточки */
    }

  /* БАЗОВЫЙ СТИЛЬ КАРТОЧКИ (ЭФФЕКТ СТЕКЛА) */
  .roadmap-content {
    background: rgba(30, 30, 30, 0.6);
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.03);
    border-radius: 16px;
    padding: 24px 28px; /* Компактные отступы внутри карточки */
    transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  }

  /* Индикаторы времени внутри карточек */
  .roadmap-date {
    font-size: 11px;
    font-weight: 700;
    color: #94a3b8;
    text-transform: uppercase;
    letter-spacing: 1.5px;
    display: block;
    margin-bottom: 10px;
  }

  .roadmap-item-title {
    font-size: 22px;
    font-weight: 700;
    color: #f8fafc;
    margin-bottom: 10px;
    transition: color 0.3s;
  }

  .roadmap-item-text {
    font-size: 15px;
    line-height: 1.7;
    color: #94a3b8;
  }

  /* Круглый значок на линии */
  .roadmap-badge {
    position: absolute;
    left: -50px;
    top: 6px;
    width: 34px;
    height: 34px;
    border-radius: 50%;
    background-color: #141414;
    border: 2px solid #94a3b8;
    color: #94a3b8;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    font-size: 13px;
    z-index: 2;
    transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  }

  /* Настройки внутренних списков (для SGP4) */
  .roadmap-item-text ul {
    list-style-type: disc !important;
    margin-top: 10px;
    padding-left: 20px;
  }

  .roadmap-item-text li {
    margin-bottom: 8px;
    color: #94a3b8;
    font-size: 14px;
  }

    .roadmap-item-text li:last-child {
      margin-bottom: 0;
    }

  /* ЭТАП ВЫПОЛНЕН И АКТИВЕН */
  .roadmap-item.completed .roadmap-badge,
  .roadmap-item.active .roadmap-badge {
    background-color: #ea75a2;
    border-color: #ea75a2;
    color: #ffffff;
    box-shadow: 0 0 20px rgba(236, 72, 153, 0.35);
  }

  .roadmap-item.completed .roadmap-content,
  .roadmap-item.active .roadmap-content {
    border-left: 3px solid #ea75a2;
  }

    /* Настройки ховера для готового и активного этапов */
    .roadmap-item.completed .roadmap-content:hover,
    .roadmap-item.active .roadmap-content:hover {
      transform: translateX(6px);
      border-color: rgba(236, 72, 153, 0.3);
      background: rgba(236, 72, 153, 0.03);
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
    }

  .roadmap-item.active .roadmap-date {
    color: #ec4899;
    text-shadow: 0 0 10px rgba(236, 72, 153, 0.2);
  }

  /* БУДУЩИЙ ЭТАП (Приглушенный стиль) */
  .roadmap-item:not(.completed):not(.active) .roadmap-content:hover {
    transform: translateX(4px);
    border-color: rgba(255, 255, 255, 0.08);
    background: rgba(40, 40, 40, 0.4);
  }

  /* Планшеты (до 1024px): уменьшаем радар, чтобы он не толкал текст */
  @media (max-width: 1024px) {
    .radar-placeholder {
      width: 240px;
      height: 240px;
    }

      .radar-placeholder::before {
        width: 110px;
        height: 110px;
      }
  }

  /* Мобильные и узкие планшеты (до 768px): hero встает в колонку,
     радар уходит под текст и уменьшается, дорожная карта сжимает отступы */
  @media (max-width: 768px) {
    .hero-section {
      min-height: auto; /* На мобильных не тянем на весь экран — контент сам определяет высоту */
      padding: 32px 0;
    }

    .hero-wrapper {
      flex-direction: column-reverse; /* Радар уходит наверх, текст остаётся легче для чтения */
      text-align: center;
      gap: 24px;
    }

    .hero-content {
      max-width: 100%;
    }

    .hero-subtitle {
      margin-bottom: 28px;
    }

    .hero-actions {
      justify-content: center;
      flex-wrap: wrap; /* Кнопки переносятся, если не помещаются в ряд */
    }

    .btn {
      flex: 1 1 auto;
      justify-content: center;
      padding: 12px 20px;
    }

    .radar-placeholder {
      width: 160px;
      height: 160px;
    }

      .radar-placeholder::before {
        width: 75px;
        height: 75px;
      }

    .roadmap-section {
      min-height: auto;
      margin-bottom: 48px;
    }

    .roadmap-box {
      padding: 32px 20px;
      border-radius: 16px;
    }

    .section-title {
      font-size: clamp(24px, 6vw, 32px);
    }

    .section-subtitle {
      font-size: 14px;
      margin-bottom: 32px;
    }

    /* Сдвигаем линию и значки таймлайна ближе к краю, чтобы карточки не сжимались */
    .roadmap-timeline {
      padding-left: 34px;
    }

    .roadmap-badge {
      left: -34px;
      width: 28px;
      height: 28px;
      font-size: 12px;
    }

    .roadmap-content {
      padding: 18px 16px;
    }

    .roadmap-item-title {
      font-size: 18px;
    }

    .roadmap-item-text,
    .roadmap-item-text li {
      font-size: 14px;
    }

    /* На тач-устройствах нет наведения — не сдвигаем карточку вбок при тапе */
    .roadmap-item.completed .roadmap-content:hover,
    .roadmap-item.active .roadmap-content:hover,
    .roadmap-item:not(.completed):not(.active) .roadmap-content:hover {
      transform: none;
    }
  }

  /* Маленькие телефоны (до 380px) */
  @media (max-width: 380px) {
    .hero-actions {
      flex-direction: column; /* Кнопки друг под другом на самых узких экранах */
      width: 100%;
    }

    .btn {
      width: 100%;
    }

    .radar-placeholder {
      width: 130px;
      height: 130px;
    }

      .radar-placeholder::before {
        width: 60px;
        height: 60px;
      }

    .roadmap-timeline {
      padding-left: 28px;
    }

    .roadmap-badge {
      left: -28px;
      width: 24px;
      height: 24px;
      font-size: 11px;
      top: 4px;
    }
  }
</style>

<script setup>
  const scrollToRoadmap = () => {
    // Находим секцию на странице
    const element = document.getElementById('roadmap')

    if (element) {
      // Считаем позицию вручную через getBoundingClientRect + window.scrollTo —
      // это работает стабильнее, чем element.scrollIntoView(), особенно на мобильных
      // браузерах (iOS Safari иногда игнорирует scrollIntoView при сложной верстке)
      const top = element.getBoundingClientRect().top + window.scrollY

      window.scrollTo({
        top,
        behavior: 'smooth'
      })
    }
  }
</script>
