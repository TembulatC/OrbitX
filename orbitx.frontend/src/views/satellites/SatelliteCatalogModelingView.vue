<template>
  <section class="catalog-section">
    <div class="container">

      <!-- 1 БЛОК — СЛОЖНАЯ ПАНЕЛЬ ФИЛЬТРОВ И ПОИСКА -->
      <div class="filters-block">

        <!-- Ряд 1: Главный поиск + отдельная маленькая квадратная кнопка поиска справа -->
        <div class="filters-row row-one">
          <div class="main-search">
            <!-- СВЯЗЬ: v-model привязывает текст к переменной searchQuery -->
            <input v-model="searchQuery" type="text" placeholder="Поиск спутника по названию или ID..." class="form-input" />
          </div>
          <!-- СВЯЗЬ: Клик по маленькой кнопке вызывает функцию fetchSatellites() -->
          <button type="button" @click="fetchSatellites" class="btn-search-small" title="Найти по названию/ID">
            🔍
          </button>
        </div>

        <!-- Ряд 2: Три выпадающих списка + Своя кнопка поиска -->
        <div class="filters-row row-two">

          <!-- 1. Выпадающий список категорий CelesTrak -->
          <div class="filter-item">
            <!-- СВЯЗЬ: v-model отслеживает выбранную категорию -->
            <select v-model="selectedCategory" class="form-select">
              <option value="all">Выбрать категорию группировки</option>
              <optgroup label="Weather & Earth Resources Satellites">
                <option value="weather">Weather</option>
                <option value="resource">Earth Resources</option>
                <option value="sar">Synthetic Aperture Radar</option>
                <option value="sarsat">Search & Rescue (SARSAT)</option>
                <option value="dmc">Disaster Monitoring</option>
                <option value="tdrss">Tracking and Data Relay Satellite System (TDRSS)</option>
                <option value="argos">ARGOS Data Collection System</option>
                <option value="planet">Planet</option>
                <option value="spire">Spire</option>
              </optgroup>
              <optgroup label="Communications Satellites">
                <option value="geo">Active Geosynchronous</option>
                <option value="gpz">GEO Protected Zone</option>
                <option value="gpz-plus">GEO Protected Zone Plus</option>
                <option value="intelsat">Intelsat</option>
                <option value="ses">SES</option>
                <option value="eutelsat">Eutelsat</option>
                <option value="telesat">Telesat</option>
                <option value="starlink">Starlink</option>
                <option value="oneweb">OneWeb</option>
                <option value="qianfan">Qianfan</option>
                <option value="hulianwang">Hulianwang Digui</option>
                <option value="kuiper">Kuiper</option>
                <option value="iridium-next">Iridium-NEXT</option>
                <option value="orbcomm">Orbcomm</option>
                <option value="globalstar">Globalstar</option>
                <option value="amateur">Amateur Radio</option>
                <option value="satnogs">SatNOGS</option>
                <option value="x-comm">Experimental Comm</option>
                <option value="other-comm">Other Comm</option>
              </optgroup>
              <optgroup label="Navigation Satellites">
                <option value="gnss">GNSS</option>
                <option value="gps-ops">GPS Operational</option>
                <option value="glo-ops">GLONASS Operational</option>
                <option value="galileo">Galileo</option>
                <option value="beidou">Beidou</option>
                <option value="sbas">Satellite-Based Augmentation System (WAAS/EGNOS/MSAS)</option>
              </optgroup>
              <optgroup label="Scientific Satellites">
                <option value="science">Space & Earth Science</option>
                <option value="geodetic">Geodetic</option>
                <option value="engineering">Engineering</option>
                <option value="education">Education</option>
              </optgroup>
              <optgroup label="Miscellaneous Satellites">
                <option value="military">Miscellaneous Military</option>
                <option value="radar">Radar Calibration</option>
                <option value="cubesat">CubeSats</option>
              </optgroup>
            </select>
          </div>

          <!-- 2. Сортировка по Id/Названию (Управляет выбором C#-метода бэкенда) -->
          <div class="filter-item">
            <!-- СВЯЗЬ: v-model привязан к sortBy (id или name) -->
            <select v-model="sortBy" class="form-select">
              <option value="id">Сортировать по NORAD ID</option>
              <option value="name">Сортировать по Названию</option>
            </select>
          </div>

          <!-- 3. Сколько отображать за раз 25/50/100 -->
          <div class="filter-item item-short">
            <!-- СВЯЗЬ: v-model привязан к переменной pageSize -->
            <select v-model="pageSize" class="form-select">
              <option value="25">25 на странице</option>
              <option value="50">50 на странице</option>
              <option value="100">100 на странице</option>
            </select>
          </div>

          <!-- 4. КНОПКА ПОИСКА ВО ВТОРОМ РЯДУ -->
          <div class="filter-action">
            <!-- СВЯЗЬ: @click запускает fetchSatellites() с проверкой категории на 'all' -->
            <button type="button" @click="fetchSatellites" class="btn-search-submit">ПОИСК</button>
          </div>

        </div>
      </div>

      <!-- 2 БЛОК — ТАБЛИЦА СПУТНИКОВ И НАВИГАЦИЯ СТРАНИЦ -->
      <div class="table-block">
        <div class="table-responsive">
          <table class="satellites-table">
            <thead>
              <tr>
                <th style="width: 25%">NoradId</th>
                <th style="width: 50%">Название спутника</th>
                <th style="width: 25%; text-align: center;">Моделирование</th>
              </tr>
            </thead>
            <tbody>
              <!-- Состояние пустоты: пока бэкенд не вернул данные, показываем заглушку -->
              <tr v-if="satellites.length === 0">
                <td colspan="3" style="text-align: center; color: #94a3b8; padding: 40px 0;">
                  Выберите категорию группировки и нажмите кнопку «Поиск» для загрузки данных
                </td>
              </tr>

              <!-- СВЯЗЬ: Цикл v-for перебирает массив satellites, прилетающий из .NET бэкенда -->
              <tr v-for="sat in satellites" :key="sat.noradId">
                <td class="td-id">{{ sat.noradId }}</td>
                <td class="td-name">{{ sat.name }}</td>
                <td class="td-action">
                  <!-- Кнопка-ссылка. Сгенерирует URL вида: /satellites_modeling?s=ID -->
                  <router-link :to="{ path: '/satellites_modeling', query: { s: sat.noradId } }"
                               class="btn-modeling">
                    Запустить
                  </router-link>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- ПАНЕЛЬ ПОСТРАНИЧНОЙ НАВИГАЦИИ (Под таблицей) -->
        <!-- Показываем её только если в таблице физически есть хотя бы один спутник -->
        <div v-if="satellites.length > 0" class="pagination-panel">
          <!-- СВЯЗЬ: Клик уменьшает страницу на 1 и шлет запрос -->
          <button type="button" @click="prevPage" class="pag-btn prev-btn" title="Предыдущая страница">‹</button>

          <div class="pag-input-wrapper">
            <!-- СВЯЗЬ: v-model связывает инпут с currentPage. При изменении через Enter сработает @change -->
            <input v-model.number="currentPage" type="number" min="1" class="pag-input" @change="fetchSatellites" title="Введите страницу и нажмите Enter" />
          </div>

          <!-- СВЯЗЬ: Клик увеличивает страницу на 1 и шлет запрос -->
          <button type="button" @click="nextPage" class="pag-btn next-btn" title="Следующая страница">›</button>
        </div>

      </div>

    </div>
  </section>

  <!-- УВЕДОМЛЕНИЕ (ВСПЛЫВАЮЩИЙ ТОАСТ) -->
  <div v-if="toast.show" class="toast-notification">
    <div class="toast-icon">📡</div>
    <div class="toast-body">
      <h5 class="toast-title">Информационное сообщение</h5>
      <p class="toast-text">{{ toast.message }}</p>
    </div>
    <button type="button" @click="closeToast" class="toast-close-btn">×</button>
  </div>
</template>



<script setup lang="ts">
  import { ref } from 'vue'

  interface Satellite {
    noradId: number
    name: string
  }

  const searchQuery = ref('')
  const selectedCategory = ref('all')
  const sortBy = ref('id')
  const pageSize = ref(25)
  const currentPage = ref(1)
  const satellites = ref<Satellite[]>([])

  // Настройка всплывающего уведомления
  const toast = ref({
    show: false,
    message: ''
  })

  let toastTimeout: number | null = null

  // Функция для вызова уведомления
  const showNotification = (msg: string) => {
    // Если уже висит старый таймер, сбрасываем его
    if (toastTimeout) clearTimeout(toastTimeout)

    toast.value.message = msg
    toast.value.show = true

    // Автоматически закрываем окно через 4 секунды (4000 мс)
    toastTimeout = window.setTimeout(() => {
      toast.value.show = false
    }, 4000)
  }

  // Ручное закрытие по крестику
  const closeToast = () => {
    toast.value.show = false
    if (toastTimeout) clearTimeout(toastTimeout)
  }

  const fetchSatellites = async () => {
    if (selectedCategory.value === 'all') {
      showNotification('Пожалуйста, выберите категорию группировки перед началом поиска!')
      return
    }

    try {
      const endpoint = sortBy.value === 'id'
        ? '/api/v1/GetSatellitesFiltersById'
        : '/api/v1/GetSatellitesFiltersByName'

      const url = new URL(`http://localhost:5000${endpoint}`)
      url.searchParams.append('category', selectedCategory.value)
      url.searchParams.append('page', currentPage.value.toString())
      url.searchParams.append('pageSize', pageSize.value.toString())

      const response = await fetch(url.toString())

      // Перехват 404 ошибки бэкенда (данные закончились)
      if (response.status === 404) {
        showNotification('Вы достигли конца списка. Дальнейших спутников в этой категории не обнаружено.')
        if (currentPage.value > 1) {
          currentPage.value--
        }
        return
      }

      if (response.ok) {
        const data = await response.json()

        // Перехват если бэкенд прислал пустой массив [] вместо 404
        if (data.length === 0) {
          showNotification('Вы достигли конца списка. Дальнейших спутников в этой категории не обнаружено.')
          if (currentPage.value > 1) {
            currentPage.value--
          }
          return
        }

        satellites.value = data
      } else {
        console.error('Ошибка бэкенда:', response.statusText)
      }
    } catch (error) {
      console.error('Не удалось связаться с сервером .NET:', error)
    }
  }

  const nextPage = () => { currentPage.value++; fetchSatellites() }
  const prevPage = () => { if (currentPage.value > 1) { currentPage.value--; fetchSatellites() } }
</script>


<style scoped>
  .catalog-section {
    background-color: #1a1a1a;
    min-height: calc(100vh - 80px);
    padding: 60px 0;
  }

  /* ==========================================================================
   БЛОК 1: СТИЛИ ПАНЕЛИ ПОИСКА И ФИЛЬТРОВ (СТРОГИЙ СТИЛЬ)
   ========================================================================= */
  .filters-block {
    background-color: #141414;
    border: 1px solid #222222;
    border-radius: 16px;
    padding: 24px;
    margin-bottom: 30px;
  }

  .filters-row {
    display: flex;
    gap: 16px;
    align-items: center;
  }

  .row-one {
    margin-bottom: 20px;
  }

  .main-search {
    flex-grow: 1;
  }

  /* Маленькая квадратная кнопка поиска из верхнего ряда */
  .btn-search-small {
    width: 42px;
    height: 42px;
    background-color: #1e1e1e;
    border: 2px solid #2d2d2d;
    border-radius: 8px;
    color: #ffffff;
    font-size: 16px;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.2s ease;
  }

    .btn-search-small:hover {
      border-color: #ea75a2;
      color: #ea75a2;
    }

  .row-two {
    display: flex;
    justify-content: space-between;
  }

  .filter-item {
    flex: 1;
  }

  .item-short {
    max-width: 180px;
  }

  /* Элементы управления (Инпуты и Селекты) */
  .form-input, .form-select {
    width: 100%;
    height: 42px;
    background-color: #1e1e1e;
    border: 2px solid #2d2d2d;
    border-radius: 8px;
    padding: 0 16px; /* Внутренний отступ добавлен, текст больше не слипается */
    color: #ffffff;
    font-size: 14px;
    outline: none;
    transition: border-color 0.2s ease;
    font-family: "Exo 2", sans-serif;
  }

  .form-select {
    cursor: pointer;
    appearance: none;
    padding-right: 40px;
    font-family: "Exo 2", sans-serif;
  }

    .form-select option, .form-select optgroup {
      background-color: #141414;
      color: #ffffff;
    }

    .form-input:focus, .form-select:focus {
      border-color: #ea75a2;
    }

  /* Большая прямоугольная кнопка "Поиск" во втором ряду */
  .btn-search-submit {
    height: 42px;
    padding: 0 28px;
    background-color: #ea75a2;
    border: 1px solid #ea75a2;
    border-radius: 8px;
    color: #ffffff;
    font-size: 15px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;
    margin-top: 1px; /* Выравнивание кнопки по сетке */
    font-family: "Exo 2", sans-serif;
  }

    .btn-search-submit:hover {
      background-color: #ec4899;
      border-color: #ec4899;
      /* НЕОН УБРАН: нет box-shadow */
    }

  /* ==========================================================================
   БЛОК 2: СТИЛИ ТАБЛИЦЫ И ПАНЕЛИ СТРАНИЦ
   ========================================================================== */
  .table-block {
    background-color: #141414;
    border: 1px solid #222222;
    border-radius: 16px;
    padding: 8px;
    overflow: hidden;
  }

  .table-responsive {
    width: 100%;
    overflow-x: auto;
  }

  .satellites-table {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
  }

    .satellites-table th {
      background-color: rgba(255, 255, 255, 0.01);
      padding: 18px 24px;
      font-size: 13px;
      font-weight: 700;
      color: #94a3b8;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      border-bottom: 2px solid #222222;
    }

    .satellites-table td {
      padding: 16px 24px;
      border-bottom: 1px solid #1f1f1f;
      font-size: 15px;
      color: #e2e8f0;
      vertical-align: middle;
    }

    .satellites-table tr:last-child td {
      border-bottom: none;
    }

    .satellites-table tr:hover td {
      background-color: rgba(255, 255, 255, 0.01);
    }

  .td-id {
    font-family: "Exo 2", sans-serif;
    color: #94a3b8;
  }

  .td-name {
    font-weight: 600;
  }

  .td-action {
    text-align: center;
  }

  /* Кнопка запуска моделирования спутника */
  .btn-modeling {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    padding: 8px 20px;
    background-color: transparent;
    border: 2px solid #ea75a2;
    color: #ea75a2;
    border-radius: 6px;
    font-size: 14px;
    font-weight: 600;
    text-decoration: none;
    cursor: pointer;
    transition: all 0.2s ease;
  }

    .btn-modeling:hover {
      background-color: #ea75a2;
      color: #ffffff;
      /* НЕОН УБРАН: нет box-shadow */
    }

  /* СТИЛИ ПАНЕЛИ СТРАНИЦ (ПАГИНАЦИЯ ПОД ТАБЛИЦЕЙ) */
  .pagination-panel {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 10px;
    padding: 24px 0 16px 0;
    border-top: 1px solid #1f1f1f;
    background-color: #141414;
  }

  /* Квадратные кнопки-стрелочки < и > */
  .pag-btn {
    width: 40px;
    height: 40px;
    background-color: #1e1e1e;
    border: 2px solid #2d2d2d;
    border-radius: 8px;
    color: #ffffff;
    font-size: 18px;
    font-weight: 600;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.2s ease;
  }

    .pag-btn:hover {
      border-color: #ea75a2;
      color: #ea75a2;
    }

  /* Поле ручного ввода страницы */
  .pag-input-wrapper {
    width: 60px;
  }

  .pag-input {
    width: 100%;
    height: 40px;
    background-color: #1e1e1e;
    border: 2px solid #2d2d2d;
    border-radius: 8px;
    color: #ffffff;
    font-size: 14px;
    font-weight: 600;
    text-align: center;
    outline: none;
    transition: border-color 0.2s ease;
  }

    .pag-input:focus {
      border-color: #ea75a2;
    }

    .pag-input::-webkit-outer-spin-button,
    .pag-input::-webkit-inner-spin-button {
      -webkit-appearance: none;
      margin: 0;
    }

    .pag-input[type=number] {
      -moz-appearance: textfield;
    }

  /* Адаптивный резиновый переход под небольшие экраны */
  @media (max-width: 768px) {
    .filters-row {
      flex-direction: column;
      align-items: stretch;
      gap: 15px;
    }

    .row-one {
      margin-bottom: 15px;
    }

    .item-short {
      max-width: 100%;
    }

    .btn-search-small, .btn-search-submit {
      width: 100%;
    }
  }

  .toast-notification {
    position: fixed;
    bottom: 40px;
    right: 40px; /* Размещаем в правом нижнем углу ПК-экрана */
    max-width: 360px;
    width: 100%;
    background-color: #141414; /* Плотный темный фон в цвет блоков */
    border: 1px solid #222222;
    border-left: 4px solid #ec4899; /* Фирменная розовая акцентная грань */
    border-radius: 12px;
    padding: 16px 20px;
    display: flex;
    align-items: flex-start;
    gap: 14px;
    z-index: 1000; /* Поверх всех таблиц и кнопок */
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5); /* Объемная тень под коробкой */
    animation: slide-in 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  }

  @keyframes slide-in {
    from {
      transform: translateX(50px);
      opacity: 0;
    }

    to {
      transform: translateX(0);
      opacity: 1;
    }
  }

  .toast-icon {
    font-size: 20px;
    line-height: 1;
    margin-top: 2px;
  }

  .toast-body {
    flex-grow: 1;
  }

  .toast-title {
    color: #ffffff;
    font-size: 14px;
    font-weight: 700;
    margin: 0 0 4px 0;
  }

  .toast-text {
    color: #94a3b8;
    font-size: 13px;
    line-height: 1.5;
    margin: 0;
  }

  /* Кнопка-крестик */
  .toast-close-btn {
    background: none;
    border: none;
    color: #64748b;
    font-size: 20px;
    line-height: 1;
    cursor: pointer;
    padding: 0;
    margin-top: -2px;
    transition: color 0.2s ease;
  }

    .toast-close-btn:hover {
      color: #ffffff;
    }
</style>
