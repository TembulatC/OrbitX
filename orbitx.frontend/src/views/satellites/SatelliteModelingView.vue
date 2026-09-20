<template>
  <section class="modeling-section">
    <div class="container">

      <!-- ВЕРХНИЙ БЛОК: ИНФОРМАЦИЯ О ТЕКУЩЕМ СПУТНИКЕ -->
      <div class="modeling-header-block">
        <div class="header-block-flex-wrapper">

          <p class="modeling-subtitle" style="margin: 0;">
            Текущий аппарат:
            <!-- Название спутника появится, как только прилетит первый пакет -->
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

        <!-- ЛЕВАЯ КОЛОНКА: КАРТА -->
        <div class="map-container-box">
          <div class="map-placeholder-content">
            <span class="placeholder-icon">🌍</span>
            <h3>Интерактивная карта траектории</h3>
            <p v-if="satelliteData" style="color: #94a3b8">
              Спутник успешно позиционирован. Ожидание подключения карты...
            </p>
            <p v-else style="color: #64748b">Ожидание первичных координат...</p>
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
            <span class="frequency-indicator"></span>
            <span style="color: #94a3b8">Обновление данных: ~1 раз / сек</span>
          </div>
        </div>

      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
  import { ref, watch, onUnmounted } from 'vue'
  import { useRoute, onBeforeRouteLeave } from 'vue-router'
  import * as signalR from '@microsoft/signalr'

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

  let hubConnection: signalR.HubConnection | null = null

  // Идентификаторы бортовых таймеров ЦУП
  let dbTimeoutTimer: number | null = null
  let heartbeatTimer: number | null = null

  const clearDbTimeout = () => {
    if (dbTimeoutTimer) {
      clearTimeout(dbTimeoutTimer)
      dbTimeoutTimer = null
    }
  }

  const clearHeartbeatTimeout = () => {
    if (heartbeatTimer) {
      clearTimeout(heartbeatTimer)
      heartbeatTimer = null
    }
  }

  /*
     Если интернет моргнул и координаты перестали идти, этот таймер сработает
     ровно через 3 секунды бездействия и вернет статус в серый режим ожидания "Подключение...",
  */
  const resetHeartbeatTimeout = () => {
    clearHeartbeatTimeout()

    heartbeatTimer = window.setTimeout(() => {
      if (syncStatus.value === 'success') {
        syncStatus.value = 'pending'
        console.warn('[ЦУП] Поток координат приостановлен. Ожидание восстановления связи...')
      }
    }, 3000)
  }

  const stopSignalR = async () => {
    clearDbTimeout()
    clearHeartbeatTimeout()

    if (hubConnection) {
      try {
        hubConnection.off('ReceivePosition')
        await hubConnection.stop()
        console.log('[SignalR] Соединение успешно разорвано.')
      } catch (err) {
        console.warn('[SignalR] Мягкий перехват при остановке группы/сокета:', err)
      } finally {
        hubConnection = null
        if ((window as any).testConnection) {
          (window as any).testConnection = null
        }
        connectionStatus.value = 'disconnected'
        satelliteData.value = null
        syncStatus.value = 'pending'
      }
    }
  }

  const startSatelliteTracking = async (id: number) => {
    await stopSignalR()

    noradId.value = id
    connectionStatus.value = 'connecting'
    syncStatus.value = 'pending'
    statusText.value = 'Установка соединения...'

    // 30-секундный таймер на случай, если лежит база данных
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
      .build(); (window as any).testConnection = hubConnection

    hubConnection.on('ReceivePosition', (data: SGP4DataDTO) => {
      clearDbTimeout()

      satelliteData.value = data

      if (syncStatus.value !== 'success') {
        syncStatus.value = 'success'
      }

      resetHeartbeatTimeout()
    })

    hubConnection.onreconnecting((error) => {
      clearDbTimeout()
      clearHeartbeatTimeout()
      syncStatus.value = 'reconnecting'
      connectionStatus.value = 'connecting'
      statusText.value = 'Переподключение...'
    })

    hubConnection.onclose((error) => {
      clearDbTimeout()
      clearHeartbeatTimeout()
      syncStatus.value = 'error'
      connectionStatus.value = 'disconnected'
      statusText.value = 'Соединение потеряно.'
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

  /* 1. Состояние ожидания (Серый) */
  .badge-pending {
    background-color: rgba(148, 163, 184, 0.02);
    border: 1px solid rgba(148, 163, 184, 0.12);
  }

    .badge-pending .status-badge-text {
      color: #94a3b8;
    }

  /* 2. Состояние успеха (Зеленый) */
  .badge-success {
    background-color: rgba(34, 197, 94, 0.03);
    border: 1px solid rgba(34, 197, 94, 0.15);
  }

    .badge-success .status-badge-text {
      color: #22c55e;
    }

  /* 3. Предупреждение (Потеря связи посреди сессии) */
  .badge-reconnecting {
    background-color: rgba(234, 179, 8, 0.03);
    border: 1px solid rgba(234, 179, 8, 0.15);
  }

    .badge-reconnecting .status-badge-text {
      color: #eab308;
    }

  /* 4. Состояние ошибки (Аварийно-красный) */
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
    padding: 24px;
    display: flex;
    flex-direction: column;
    position: relative;
  }

  .map-placeholder-content {
    flex-grow: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
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
    background-color: #22c55e;
    border-radius: 50%;
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
