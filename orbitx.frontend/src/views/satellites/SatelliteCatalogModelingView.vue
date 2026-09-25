<template>
  <section class="catalog-section">
    <div class="container">

      <!-- 1 БЛОК — ПАНЕЛЬ ФИЛЬТРОВ И ПОИСКА -->
      <div class="filters-block">

        <!-- Ряд 1: Поиск по ID/названию + кнопка-лупа -->
        <div class="filters-row row-one">
          <div class="main-search">
            <input v-model="searchQuery" type="text" placeholder="Поиск спутника по названию или ID..." class="form-input" @keyup.enter="handleTopSearch" />
          </div>
          <!-- Кнопка-поиск: определяет число это или текст и дергает нужный метод API -->
          <button type="button" class="btn-search-small" title="Найти по названию/ID" @click="handleTopSearch">
            ПОИСК
          </button>
        </div>

        <!-- Ряд 2: Три выпадающих списка + Своя кнопка поиска -->
        <div class="filters-row row-two">

          <!-- 1. Выпадающий список категорий CelesTrak -->
          <div class="filter-item">
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
                <option value="iridium-next">Iridium NEXT</option>
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

          <!-- 2. Сортировка по Id/Названию -->
          <div class="filter-item">
            <select v-model="sortBy" class="form-select">
              <option value="id">Сортировать по NORAD ID</option>
              <option value="name">Сортировать по Названию</option>
            </select>
          </div>

          <!-- 3. Сколько отображать за раз 25/50/100 -->
          <div class="filter-item item-short">
            <select v-model="pageSize" class="form-select">
              <option value="25">25 на странице</option>
              <option value="50">50 на странице</option>
              <option value="100">100 на странице</option>
            </select>
          </div>

          <!-- 4. КНОПКА ПОИСКА ВО ВТОРОМ РЯДУ -->
          <div class="filter-action">
            <button type="button" @click="handleSearch" class="btn-search-submit">ПОИСК</button>
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
              <tr v-if="satellites.length === 0">
                <td colspan="3" style="text-align: center; color: #64748b; padding: 40px 0;">
                  Выберите категорию группировки и нажмите кнопку «Поиск» для загрузки данных
                </td>
              </tr>

              <tr v-for="sat in satellites" :key="sat.noradId">
                <td class="td-id">{{ sat.noradId }}</td>
                <td class="td-name">{{ sat.name }}</td>
                <td class="td-action">
                  <router-link :to="{ name: 'satellites-modeling', params: { satellite_id: sat.noradId } }" class="btn-modeling">
                    Запустить
                  </router-link>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- ПАНЕЛЬ ПОСТРАНИЧНОЙ НАВИГАЦИИ -->
        <div v-if="satellites.length > 0" class="pagination-panel">
          <button type="button" @click="prevPage" class="pag-btn prev-btn" title="Предыдущая страница">‹</button>
          <div class="pag-input-wrapper">
            <!-- При ручном вводе передаем текущее вбитое число -->
            <input v-model.number="currentPage" type="number" min="1" class="pag-input" @change="fetchSatellites(currentPage)" title="Введите страницу и нажмите Enter" />
          </div>
          <button type="button" @click="nextPage" class="pag-btn next-btn" title="Следующая страница">›</button>
        </div>

      </div>

    </div>

    <!-- УВЕДОМЛЕНИЯ ОТ БЕКЕНДА -->
    <div v-if="toast.show" class="toast-notification">
      <div class="toast-icon">📡</div>
      <div class="toast-body">
        <h5 class="toast-title">Информационное сообщение</h5>
        <p class="toast-text">{{ toast.message }}</p>
      </div>
      <button type="button" @click="closeToast" class="toast-close-btn">×</button>
    </div>
  </section>
</template>

<script setup lang="ts">
  import { ref } from 'vue'

  // Описание интерфейса объекта спутника
  interface Satellite {
    noradId: number
    name: string
  }

  // 1. Переменные для двусторонней связи (v-model) с HTML-полями
  const searchQuery = ref('')         // Строка поиска по ключевым словам (верхнее поле: ID или имя)
  const selectedCategory = ref('all')   // Категория группировки (изначально "all")
  const sortBy = ref('id')             // Метод сортировки: 'id' (для FiltersById) или 'name' (для FiltersByName)
  const pageSize = ref(25)            // Количество строк на странице (по умолчанию 25)
  const currentPage = ref(1)          // Номер текущей страницы пагинации

  // Массив спутников, полученный с бэкенда (изначально пустой)
  const satellites = ref<Satellite[]>([])

  // Состояние всплывающего уведомления
  const toast = ref({
    show: false,
    message: ''
  })

  let toastTimeout: number | null = null

  // Функция для вызова всплывающего окна уведомления
  const showNotification = (msg: string) => {
    if (toastTimeout) clearTimeout(toastTimeout)
    toast.value.message = msg
    toast.value.show = true
    toastTimeout = window.setTimeout(() => {
      toast.value.show = false
    }, 4000)
  }

  // Функция для ручного закрытия уведомления по крестику
  const closeToast = () => {
    toast.value.show = false
    if (toastTimeout) clearTimeout(toastTimeout)
  }

  // Ответ бэкенда содержит больше полей, чем нужно таблице (долгота/широта/высота)
  interface SGP4DataDTO {
    noradId: number
    name: string
    longitude?: number
    latitude?: number
    altitude?: number
  }

  // Берём из DTO только то, что реально нужно таблице
  const mapToSatellite = (dto: SGP4DataDTO): Satellite => ({
    noradId: dto.noradId,
    name: dto.name
  })

  // Проверка: можно ли строку целиком превратить в число (NORAD ID)
  // Пустая строка и строки с "мусором" числом не считаются
  const isNoradId = (value: string): boolean => {
    if (value.trim() === '') return false
    return !isNaN(Number(value)) && !isNaN(parseFloat(value))
  }

  // Главный обработчик верхнего поиска
  const handleTopSearch = async () => {
    const query = searchQuery.value.trim()

    if (query === '') {
      showNotification('Введите название спутника или NORAD ID для поиска')
      return
    }

    currentPage.value = 1

    if (isNoradId(query)) {
      await fetchSatelliteById(Number(query))
    } else {
      await fetchSatelliteByName(query)
    }
  }

  // Запрос спутника по NORAD ID → GetDataById
  const fetchSatelliteById = async (noradId: number) => {
    try {
      const url = new URL(`${window.location.origin}/api/v1/GetDataById`)
      url.searchParams.append('noradId', noradId.toString())

      const response = await fetch(url.toString())

      if (response.status === 404) {
        showNotification(`Спутник с NORAD ID ${noradId} не найден`)
        satellites.value = []
        return
      }

      if (response.ok) {
        const data = await response.json()
        // Бэкенд может вернуть как один объект, так и массив — приводим к единому виду
        const list: SGP4DataDTO[] = Array.isArray(data) ? data : [data]
        satellites.value = list.map(mapToSatellite)
        return
      }

      // Любой другой неуспешный статус (400, 500 и т.д.) — показываем явно
      const bodyText = await response.text().catch(() => '')
      console.error('Ошибка бэкенда:', response.status, response.statusText, bodyText)
      showNotification(`Не удалось получить данные о спутнике (код ${response.status})`)
    } catch (error) {
      console.error('Не удалось связаться с сервером', error)
      showNotification('Ошибка соединения с сервером')
    }
  }

  // Запрос спутника по названию → GetDataByName
  const fetchSatelliteByName = async (name: string) => {
    try {
      const url = new URL(`${window.location.origin}/api/v1/GetDataByName`)
      url.searchParams.append('satelliteName', name)

      const response = await fetch(url.toString())

      if (response.status === 404) {
        showNotification(`Спутник «${name}» не найден.`)
        satellites.value = []
        return
      }

      if (response.ok) {
        const data = await response.json()
        const list: SGP4DataDTO[] = Array.isArray(data) ? data : [data]
        satellites.value = list.map(mapToSatellite)
        return
      }

      const bodyText = await response.text().catch(() => '')
      console.error('Ошибка бэкенда:', response.status, response.statusText, bodyText)
      showNotification(`Не удалось получить данные о спутнике (код ${response.status}).`)
    } catch (error) {
      console.error('Не удалось связаться с сервером .NET:', error)
      showNotification('Ошибка соединения с сервером.')
    }
  }

  // 2. Функция первичного поиска (По большой розовой кнопке)
  const handleSearch = () => {
    if (selectedCategory.value === 'all') {
      showNotification('Пожалуйста, выберите категорию спутников перед началом поиска')
      return
    }

    // Сначала синхронно сбрасываем инпут на 1 во Vue для визуального отображения
    currentPage.value = 1

    // Вызываем сетевой метод и заставляем его слать цифру 1 в аргументе
    fetchSatellites(1)
  }

  // 3. Главная функция запроса
  // Она принимает точный номер страницы, который нужно отправить в базу данных
  const fetchSatellites = async (targetPage: number = currentPage.value) => {
    try {
      const endpoint = sortBy.value === 'id'
        ? '/api/v1/GetSatellitesFiltersById'
        : '/api/v1/GetSatellitesFiltersByName'

      const url = new URL(`${window.location.origin}${endpoint}`)
      url.searchParams.append('category', selectedCategory.value)

      /*
         Мы шлем строго то число, которое пришло в аргументе функции (targetPage)
         Если вызван поиск, там будет чистая цифра 1. Vue не сможет подсунуть другую страницу
      */
      url.searchParams.append('page', targetPage.toString())
      url.searchParams.append('pageSize', pageSize.value.toString())

      const response = await fetch(url.toString())

      if (response.status === 404) {
        showNotification('Вы достигли конца списка. Дальнейших спутников в этой категории не обнаружено')
        if (currentPage.value > 1) {
          currentPage.value--
        }
        return
      }

      if (response.ok) {
        const data = await response.json()

        if (data.length === 0) {
          showNotification('Вы достигли конца списка. Дальнейших спутников в этой категории не обнаружено')
          if (currentPage.value > 1) {
            currentPage.value--
          }
          return
        }

        satellites.value = data
        return
      }

      // Любой другой неуспешный статус (400, 500 и т.д.) — показываем явно
      const bodyText = await response.text().catch(() => '')
      console.error('Ошибка бэкенда:', response.status, response.statusText, bodyText)
      showNotification(`Не удалось загрузить список спутников (код ${response.status})`)
    } catch (error) {
      console.error('Не удалось связаться с сервером', error)
      showNotification('Ошибка соединения с сервером')
    }
  }

  // Методы пагинации передают измененное состояние в аргумент явно
  const nextPage = () => {
    currentPage.value++
    fetchSatellites(currentPage.value)
  }

  const prevPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--
      fetchSatellites(currentPage.value)
    }
  }
</script>

<style scoped>
  .catalog-section {
    background-color: #1a1a1a;
    min-height: calc(100vh - 80px);
    padding: 60px 0;
  }

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

  .btn-search-small {
    color: #fff;
    cursor: pointer;
    background-color: #1e1e1e;
    border: 2px solid #2d2d2d;
    border-radius: 8px;
    justify-content: center;
    align-items: center;
    width: 107.88px;
    height: 42px;
    font-size: 16px;
    transition: all .2s;
    display: flex;
    padding-bottom: 1px;
    font-family: "Exo 2", sans-serif;
    font-weight: 600;
    touch-action: manipulation; /* Убирает 300мс задержку тапа и случайный зум по двойному тапу на мобильных */
  }

    .btn-search-small:hover,
    .btn-search-small:active {
      border-color: #ea75a2;
      color: #ea75a2;
    }

    /* Явный отклик при нажатии */
    .btn-search-small:active {
      background-color: #232323;
      transform: scale(0.97);
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
    padding: 0 16px;
    color: #ffffff;
    font-size: 14px;
    outline: none;
    transition: border-color 0.2s ease;
    font-family: "Exo 2", sans-serif;
    touch-action: manipulation; /* Быстрее реагируют на тап, без задержки под двойной тап-зум */
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
    margin-top: 1px;
    font-family: "Exo 2", sans-serif;
    touch-action: manipulation;
  }

    .btn-search-submit:hover,
    .btn-search-submit:active {
      background-color: #ec4899;
      border-color: #ec4899;
    }

    .btn-search-submit:active {
      transform: scale(0.97);
    }

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
    touch-action: manipulation;
  }

    .btn-modeling:hover,
    .btn-modeling:active {
      background-color: #ea75a2;
      color: #ffffff;
    }

  .pagination-panel {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 10px;
    padding: 24px 0 16px 0;
    border-top: 1px solid #1f1f1f;
    background-color: #141414;
  }

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
    touch-action: manipulation;
  }

    .pag-btn:hover,
    .pag-btn:active {
      border-color: #ea75a2;
      color: #ea75a2;
    }

    .pag-btn:active {
      transform: scale(0.94);
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

  /* Переход под небольшие экраны */
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

    /* На iOS Safari инпут/селект с font-size меньше 16px заставляет браузер
       автоматически зумить страницу при фокусе — из-за этого кажется, что
       элементы "уезжают" и перестают откликаться на нажатия. Держим 16px. */
    .form-input, .form-select {
      font-size: 16px;
      height: 46px; /* Чуть выше — удобнее попадать пальцем */
    }

    .btn-search-small,
    .btn-search-submit {
      height: 46px;
    }

    /* Увеличиваем зону нажатия у кнопок пагинации до рекомендуемых ~44px */
    .pag-btn {
      width: 44px;
      height: 44px;
    }

    .pag-input {
      height: 44px;
      font-size: 16px;
    }

    /* Ссылка-кнопка "Запустить" в таблице — увеличиваем зону нажатия */
    .btn-modeling {
      padding: 10px 18px;
    }
  }

  @media (max-width: 480px) {
    .catalog-section {
      padding: 32px 0;
    }

    .filters-block {
      padding: 16px;
    }

    .satellites-table th,
    .satellites-table td {
      padding: 12px 14px;
      font-size: 13px;
    }

    .btn-modeling {
      padding: 8px 14px;
      font-size: 13px;
    }
  }

  .toast-notification {
    position: fixed;
    bottom: 40px;
    right: 40px;
    max-width: 360px;
    width: 100%;
    background-color: #141414;
    border: 1px solid #222222;
    border-left: 4px solid #cf4444;
    border-radius: 12px;
    padding: 16px 20px;
    display: flex;
    align-items: flex-start;
    gap: 14px;
    z-index: 1000; /* Поверх всех таблиц и кнопок */
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5); /* Объемная тень под коробкой */
    animation: slide-in 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  }

  /* На узких экранах уведомление занимает всю ширину с отступами —
     иначе при right: 40px оно частично уезжало за левый край экрана
     (в котором лежала часть текста, а зона под кнопку "×" сжималась) */
  @media (max-width: 480px) {
    .toast-notification {
      left: 16px;
      right: 16px;
      bottom: calc(16px + env(safe-area-inset-bottom, 0px));
      max-width: none;
      width: auto;
    }
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
    padding: 8px; /* Увеличенная зона нажатия под палец, визуально компенсируем отрицательным отступом */
    margin: -8px -8px -8px 0;
    touch-action: manipulation;
    transition: color 0.2s ease;
  }

    .toast-close-btn:hover,
    .toast-close-btn:active {
      color: #ffffff;
    }
</style>
