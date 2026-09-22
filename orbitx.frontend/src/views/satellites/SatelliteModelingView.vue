<template>
  <section class="modeling-section">
    <div class="container">

      <!-- ВЕРХНИЙ БЛОК: ИНФОРМАЦИЯ О ТЕКУЩЕМ СПУТНИКЕ -->
      <div class="modeling-header-block">
        <div class="header-block-flex-wrapper">

          <p class="modeling-subtitle" style="margin: 0;">
            Текущий аппарат:
            <span class="accent-text name-accent" v-if="satelliteData">{{ satelliteData.name }}</span>
            <span class="accent-text" v-else>Определение объекта...</span>
            | NORAD ID: <span class="accent-id">{{ noradId }}</span>
          </p>

          <!-- ИНДИКАТОР ПОДКЛЮЧЕНИЙ -->
          <div class="sync-status-badge" :class="(syncStatus === 'error' || syncStatus === 'db_error') ? 'badge-error' : `badge-${syncStatus}`">
            <span class="status-dot" :class="(syncStatus === 'error' || syncStatus === 'db_error') ? 'dot-error' : `dot-${syncStatus}`"></span>
            <p class="status-badge-text">
              <template v-if="syncStatus === 'pending'">
                Подключение...
              </template>
              <template v-else-if="syncStatus === 'success'">
                Моделирование подключено
              </template>
              <template v-else-if="syncStatus === 'reconnecting'">
                Потеря связи. Пожалуйста, перезапустите моделирование
              </template>
              <template v-else-if="syncStatus === 'error'">
                Ошибка подключения. Сервер временно не отвечает.
              </template>
              <template v-else-if="syncStatus === 'db_error'">
                Ошибка подключения. Сервер временно не отвечает.
              </template>
            </p>
          </div>

        </div>
      </div>

      <!-- ОСНОВНАЯ СЕТКА ПК-ИНТЕРФЕЙСА -->
      <div class="modeling-grid">

        <!-- ЛЕВАЯ КОЛОНКА: ИНТЕРАКТИВНАЯ КАРТА LEAFLET -->
        <div class="map-container-box">

          <!-- Главный контейнер для Leaflet.js -->
          <div id="orbitx-leaflet-map"
               class="real-map-element"
               :class="{ 'map-visible': isMapLoaded }"></div>

          <!-- ПЛЕЙСХОЛДЕР -->
          <div class="map-placeholder-content" v-if="!isMapLoaded">
            <span class="placeholder-icon">🌍</span>
            <h3>Interactive Trajectory Map</h3>
            <p style="color: #64748b">Загрузка карты...</p>
          </div>

          <!-- УВЕДОМЛЕНИЕ: Высвечивается поверх карты, пока SignalR не прислал первую точку -->
          <div class="satellite-wait-toast" v-if="isMapLoaded && !satelliteData">
            <span class="toast-spinner"></span>
            <p class="toast-text">Карта готова. Инициализация координат...</p>
          </div>

        </div>

        <!-- ПРАВАЯ КОЛОНКА: ПАНЕЛЬ МЕСТОПОЛОЖЕНИЯ АППАРАТА -->
        <div class="telemetry-sidebar">
          <h3 class="sidebar-title">Местоположение аппарата</h3>

          <div class="telemetry-grid">
            <!-- 1. Широта (Latitude) -->
            <div class="telemetry-card">
              <span class="telemetry-label">Широта (Latitude)</span>
              <span class="telemetry-value" v-if="satelliteData">
                {{ satelliteData.latitude?.toFixed(6) }}°
              </span>
              <span class="telemetry-value loading-value" v-else>--.------</span>
            </div>

            <!-- 2. Долгота (Longitude) -->
            <div class="telemetry-card">
              <span class="telemetry-label">Долгота (Longitude)</span>
              <span class="telemetry-value" v-if="satelliteData">
                {{ satelliteData.longitude?.toFixed(6) }}°
              </span>
              <span class="telemetry-value loading-value" v-else>--.------</span>
            </div>

            <!-- 3. Высота (Altitude) -->
            <div class="telemetry-card">
              <span class="telemetry-label">Высота (Altitude)</span>
              <span class="telemetry-value value-alt" v-if="satelliteData">
                {{ satelliteData.altitude?.toFixed(2) }} км
              </span>
              <span class="telemetry-value loading-value" v-else>---.--</span>
            </div>
          </div>

          <!-- Техническая плашка частоты обновления пакетов -->
          <div class="update-frequency-box" v-if="satelliteData">
            <span class="frequency-indicator"
                  :class="syncStatus === 'success' ? 'indicator-success' : 'indicator-error blink-dot'"></span>
            <span style="color: #94a3b8">
              Обновление данных: {{ syncStatus === 'success' ? '~1 раз / сек' : '~0 раз / сек' }}
            </span>
          </div>
        </div>

      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
  import { ref, watch, onUnmounted, nextTick } from 'vue'
  import { useRoute } from 'vue-router'
  import { onBeforeRouteLeave } from 'vue-router'
  import * as signalR from '@microsoft/signalr'
  import L from 'leaflet'

  interface SGP4DataDTO {
    noradId: number
    name: string
    longitude: number
    latitude: number
    altitude: number
  }

  const route = useRoute()

  const noradId = ref<number>(0)
  const satelliteData = ref<SGP4DataDTO | null>(null)
  const syncStatus = ref<'pending' | 'success' | 'reconnecting' | 'error' | 'db_error'>('pending')
  const connectionStatus = ref<'connecting' | 'connected' | 'disconnected'>('connecting')
  const statusText = ref('Установка соединения...')

  const isMapLoaded = ref<boolean>(false)
  let map: L.Map | null = null
  let satelliteMarker: L.Marker | null = null
  let orbitPath: L.Polyline | null = null

  const pathCoordinates = ref<L.LatLngExpression[]>([])
  let hubConnection: signalR.HubConnection | null = null
  let dbTimeoutTimer: number | null = null
  let heartbeatTimer: number | null = null
  let isFirstPosition = true

  // Хранилище для отслеживания долготы предыдущей полученной точки спутника
  let prevLongitude: number | null = null

  const initLeafletMap = () => {
    if (map) {
      map.remove()
      map = null
      satelliteMarker = null
      orbitPath = null
    }

    isMapLoaded.value = true

    // Даем Vue один тик на обновление DOM-дерева и очистку плейсхолдера
    nextTick(() => {
      // Ограничиваем область карты максимальными координатами планеты
      const corner1 = L.latLng(-90, -180)
      const corner2 = L.latLng(90, 180)
      const bounds = L.latLngBounds(corner1, corner2)

      map = L.map('orbitx-leaflet-map', {
        center: [0, 0],
        zoom: 2,
        minZoom: 1,
        maxZoom: 15,
        zoomControl: true,
        maxBounds: bounds,
        maxBoundsViscosity: 1.0
      })

      L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
      }).addTo(map);

      orbitPath = L.polyline([], {
        color: '#ea75a2',
        weight: 3,
        opacity: 0.8,
        dashArray: '5, 5'
      }).addTo(map)

      // Фикс для дефолтных картинок маркеров Leaflet в Vite
      delete (L.Icon.Default.prototype as any)._getIconUrl
      L.Icon.Default.mergeOptions({
        iconRetinaUrl: new URL('leaflet/dist/images/marker-icon-2x.png', import.meta.url).href,
        iconUrl: new URL('leaflet/dist/images/marker-icon.png', import.meta.url).href,
        shadowUrl: new URL('leaflet/dist/images/marker-shadow.png', import.meta.url).href
      })

      // Принудительный пересчет размеров контейнера через таймаут
      setTimeout(() => {
        if (map) {
          map.invalidateSize()
          map.setView([0, 0], 2)
        }
      }, 100)
    })
  }

  const updateSatelliteOnMap = (lat: number, lng: number) => {
    if (!map) return

    const newPos = L.latLng(lat, lng)

    if (!satelliteMarker) {
      const satelliteIcon = L.divIcon({
        className: 'custom-satellite-icon',
        html: `<div class="satellite-ping-core"></div><div class="satellite-ping-wave"></div>`,
        iconSize: [15, 15],
        iconAnchor: [7.5, 7.5]
      })

      satelliteMarker = L.marker(newPos, { icon: satelliteIcon }).addTo(map)
    } else {
      satelliteMarker.setLatLng(newPos)
    }

    if (prevLongitude !== null) {
      // Если дельта по долготе больше 180 градусов, значит произошел переход через край карты (180°/-180°)
      // Спутник пошел на новый круг, полностью очищаем накопленные координаты траектории
      if (Math.abs(lng - prevLongitude) > 180) {
        pathCoordinates.value = []
        console.log('[ЦУП] Спутник завершил виток. Траектория сброшена для нового круга.')
      }
    }
    // Обновляем значение долготы текущим шагом
    prevLongitude = lng

    pathCoordinates.value.push(newPos)

    if (orbitPath) {
      orbitPath.setLatLngs(pathCoordinates.value)
    }

    if (isFirstPosition) {
      map.setView(newPos, 3, { animate: true })
      isFirstPosition = false
    }
  }

  const clearDbTimeout = () => {
    if (dbTimeoutTimer) { clearTimeout(dbTimeoutTimer); dbTimeoutTimer = null; }
  }

  const clearHeartbeatTimeout = () => {
    if (heartbeatTimer) { clearTimeout(heartbeatTimer); heartbeatTimer = null; }
  }

  const resetHeartbeatTimeout = () => {
    clearHeartbeatTimeout()
    heartbeatTimer = window.setTimeout(() => {
      if (syncStatus.value === 'success') {
        syncStatus.value = 'pending'
        console.warn('[ЦУП] Поток координат приостановлен. Ожидание восстановления связи...')
      }
    }, 4000)
  }

  const stopSignalR = async () => {
    clearDbTimeout()
    clearHeartbeatTimeout()
    pathCoordinates.value = []
    isFirstPosition = true

    if (hubConnection) {
      try {
        hubConnection.off('ReceivePosition')
        await hubConnection.stop()
        console.log('[SignalR] Соединение успешно разорвано.')
      } catch (err) {
        console.warn('[SignalR] Мягкий перехват при остановке группы/сокета:', err)
      } finally {
        hubConnection = null
        connectionStatus.value = 'disconnected'
        satelliteData.value = null
        syncStatus.value = 'pending'
      }
    }
  }

  const startSatelliteTracking = async (id: number) => {
    await stopSignalR()
    await nextTick()

    initLeafletMap()

    noradId.value = id
    connectionStatus.value = 'connecting'
    syncStatus.value = 'pending'
    statusText.value = 'Установка соединения...'

    dbTimeoutTimer = window.setTimeout(() => {
      if (syncStatus.value === 'pending') {
        syncStatus.value = 'db_error'
        console.warn('[ЦУП] Превышено время ожидания ответа от базы данных.')
      }
    }, 30000)

    hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${window.location.origin}/ws/satellite`)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build()

    hubConnection.on('ReceivePosition', (data: SGP4DataDTO) => {
      clearDbTimeout()
      satelliteData.value = data

      if (syncStatus.value !== 'success') {
        syncStatus.value = 'success'
      }

      updateSatelliteOnMap(data.latitude, data.longitude)
      resetHeartbeatTimeout()
    })

    hubConnection.onreconnecting(() => {
      clearDbTimeout()
      clearHeartbeatTimeout()
      syncStatus.value = 'reconnecting'
      connectionStatus.value = 'connecting'
      statusText.value = 'Переподключение...'
      satelliteData.value = null
    })

    hubConnection.onclose(() => {
      clearDbTimeout()
      clearHeartbeatTimeout()
      syncStatus.value = 'error'
      connectionStatus.value = 'disconnected'
      statusText.value = 'Соединение потеряно.'
      satelliteData.value = null
    })

    try {
      await hubConnection.start()
      connectionStatus.value = 'connected'
      statusText.value = 'Моделирование подключено'
      await hubConnection.invoke('WatchSatellite', id)
      resetHeartbeatTimeout()
    } catch (err) {
      clearDbTimeout()
      clearHeartbeatTimeout()
      syncStatus.value = 'error'
      connectionStatus.value = 'disconnected'
      statusText.value = 'Ошибка подключения. Возможно не отвечает сервер.'
    }
  }

  onBeforeRouteLeave(async (to, from, next) => {
    await stopSignalR()
    next()
  })

  watch(
    () => route.params.satellite_id,
    (newId) => {
      if (newId) {
        const parsedId = parseInt(newId as string, 10)
        if (!isNaN(parsedId)) {
          startSatelliteTracking(parsedId)
        }
      }
    },
    { immediate: true }
  )

  onUnmounted(() => {
    stopSignalR()
    if (map) {
      map.remove()
    }
  })
</script>

<style scoped>
  .modeling-section {
    background-color: #1a1a1a;
    min-height: calc(100vh - 80px);
    padding: 40px 0;
  }

  .modeling-header-block {
    background-color: #141414;
    border: 1px solid #222222;
    border-radius: 16px;
    padding: 24px;
    margin-bottom: 30px;
  }

  .header-block-flex-wrapper {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
  }

  .modeling-subtitle {
    color: #94a3b8;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }

  .accent-text {
    color: #ffffff;
    text-transform: none;
  }

  .name-accent {
    color: #ea75a2;
  }

  .accent-id {
    color: #ea75a2;
    font-family: monospace;
    font-size: 20px;
    font-weight: 700;
    background-color: rgba(234, 117, 162, 0.05);
    padding: 2px 8px;
    border-radius: 4px;
    border: 1px solid rgba(234, 117, 162, 0.15);
  }

  .sync-status-badge {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    padding: 6px 14px;
    border-radius: 20px;
    height: 32px;
    transition: all 0.3s ease;
  }

  .badge-pending {
    background-color: rgba(148, 163, 184, 0.02);
    border: 1px solid rgba(148, 163, 184, 0.12);
  }

    .badge-pending .status-badge-text {
      color: #94a3b8;
    }

  .badge-success {
    background-color: rgba(34, 197, 94, 0.03);
    border: 1px solid rgba(34, 197, 94, 0.15);
  }

    .badge-success .status-badge-text {
      color: #22c55e;
    }

  .badge-reconnecting {
    background-color: rgba(234, 179, 8, 0.03);
    border: 1px solid rgba(234, 179, 8, 0.15);
  }

    .badge-reconnecting .status-badge-text {
      color: #eab308;
    }

  .badge-error {
    background-color: rgba(239, 68, 68, 0.03);
    border: 1px solid rgba(239, 68, 68, 0.15);
  }

    .badge-error .status-badge-text {
      color: #ef4444;
    }

  .status-dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
  }

  .dot-pending {
    background-color: #94a3b8;
    animation: pulse-animation 1.5s infinite ease-in-out;
  }

  .dot-success {
    background-color: #22c55e;
    animation: pulse-animation 1.5s infinite ease-in-out;
  }

  .dot-reconnecting {
    background-color: #eab308;
    animation: pulse-animation 1.2s infinite ease-in-out;
  }

  .dot-error {
    background-color: #ef4444;
    animation: pulse-animation 0.8s infinite ease-in-out;
  }

  .status-badge-text {
    font-size: 13px;
    font-weight: 600;
    margin: 0;
    white-space: nowrap;
  }

  .modeling-grid {
    display: flex;
    gap: 24px;
  }

  .map-container-box {
    flex: 1;
    background-color: #141414;
    border: 1px solid #222222;
    border-radius: 16px;
    height: 550px;
    position: relative;
    overflow: hidden;
  }

  .real-map-element {
    width: 100%;
    height: 100%;
    opacity: 1 !important;
    visibility: visible !important;
  }

  .map-placeholder-content {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 2;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    width: 100%;
  }

  .placeholder-icon {
    font-size: 54px;
    margin-bottom: 16px;
  }

  .map-placeholder-content h3 {
    color: #ffffff;
    font-size: 18px;
    font-weight: 700;
    margin-bottom: 10px;
  }

  .custom-satellite-icon {
    position: relative;
  }

  .satellite-ping-core {
    width: 10px;
    height: 10px;
    background-color: #ea75a2;
    border: 2px solid #ffffff;
    border-radius: 50%;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 5;
    box-shadow: 0 0 10px #ea75a2;
  }

  .satellite-ping-wave {
    width: 30px;
    height: 30px;
    border: 2px solid #ea75a2;
    border-radius: 50%;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    animation: satellite-pulse 1.8s infinite ease-out;
    opacity: 0;
  }

  @keyframes satellite-pulse {
    0% {
      transform: translate(-50%, -50%) scale(0.3);
      opacity: 0.8;
    }

    100% {
      transform: translate(-50%, -50%) scale(1.5);
      opacity: 0;
    }
  }

  .satellite-wait-toast {
    position: absolute;
    bottom: 20px;
    left: 50%;
    transform: translateX(-50%);
    background-color: rgba(20, 20, 20, 0.9);
    border: 1px solid rgba(234, 117, 162, 0.3);
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.6);
    padding: 12px 20px;
    border-radius: 30px;
    display: flex;
    align-items: center;
    gap: 12px;
    z-index: 10;
    pointer-events: none;
    animation: fade-in-up 0.4s ease forwards;
  }

  .toast-text {
    margin: 0;
    font-size: 12px;
    font-weight: 600;
    color: #e2e8f0;
    white-space: nowrap;
  }

  .toast-spinner {
    width: 14px;
    height: 14px;
    border: 2px solid rgba(234, 117, 162, 0.2);
    border-top-color: #ea75a2;
    border-radius: 50%;
    animation: toast-spin 0.8s linear infinite;
  }

  @keyframes toast-spin {
    to {
      transform: rotate(360deg);
    }
  }

  @keyframes fade-in-up {
    from {
      opacity: 0;
      transform: translate(-50%, 10px);
    }

    to {
      opacity: 1;
      transform: translate(-50%, 0);
    }
  }

  .telemetry-sidebar {
    width: 340px;
    background-color: #141414;
    border: 1px solid #222222;
    border-radius: 16px;
    padding: 24px;
    display: flex;
    flex-direction: column;
  }

  .sidebar-title {
    color: #94a3b8;
    font-size: 13px;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    margin-bottom: 20px;
    border-bottom: 1px solid #222222;
    padding-bottom: 12px;
  }

  .telemetry-grid {
    display: flex;
    flex-direction: column;
    gap: 16px;
    flex-grow: 1;
  }

  .telemetry-card {
    background-color: #1e1e1e;
    border: 1px solid #2d2d2d;
    border-radius: 12px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 6px;
  }

  .telemetry-label {
    font-size: 11px;
    font-weight: 700;
    color: #64748b;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }

  .telemetry-value {
    color: #ffffff;
    font-size: 22px;
    font-weight: 700;
    font-family: monospace;
  }

  .value-alt {
    color: #ea75a2;
  }

  .loading-value {
    color: #2d2d2d;
  }

  .update-frequency-box {
    margin-top: 20px;
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 11px;
    color: #64748b;
    background-color: rgba(255, 255, 255, 0.01);
    padding: 8px 12px;
    border-radius: 6px;
    border: 1px solid rgba(255, 255, 255, 0.02);
  }

  .frequency-indicator {
    width: 6px;
    height: 6px;
    border-radius: 50%;
    transition: background-color 0.3s ease;
  }

  .indicator-success {
    background-color: #22c55e;
  }

  .indicator-error {
    background-color: #ef4444;
  }

  .blink-dot {
    animation: pulse-animation 0.8s infinite ease-in-out;
  }

  @keyframes pulse-animation {
    0% {
      transform: scale(0.9);
      opacity: 0.5;
    }

    50% {
      transform: scale(1.05);
      opacity: 1;
    }

    100% {
      transform: scale(0.9);
      opacity: 0.5;
    }
  }

  @media (max-width: 992px) {
    .header-block-flex-wrapper {
      flex-direction: column;
      align-items: flex-start;
      gap: 12px;
    }

    .modeling-grid {
      flex-direction: column;
    }

    .telemetry-sidebar {
      width: 100%;
    }

    .map-container-box {
      height: 400px;
    }
  }
</style>

<!-- Отдельный неизолированный тег для стилизации элементов Leaflet -->
<style>
  @import "leaflet/dist/leaflet.css";

  /* Базовый цвет подложки — черный */
  .leaflet-container {
    background-color: #000000 !important;
  }

  .leaflet-tile-container img {
    filter: invert(100%) hue-rotate(180deg) brightness(95%) contrast(90%) saturate(30%);
  }

  /* Сокрытие строки копирайта */
  .leaflet-control-attribution {
    display: none !important;
  }

  /* Тёмные кнопки зума */
  .leaflet-control-zoom a {
    color: #ffffff !important;
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
    text-decoration: none !important;
    background-color: #141414 !important;
    border: 1px solid #222222 !important;
    border-bottom: none !important;
  }

    .leaflet-control-zoom a:last-child {
      border-bottom: 1px solid #222222 !important;
    }

    .leaflet-control-zoom a:hover {
      background-color: #1e1e1e !important;
    }
</style>
