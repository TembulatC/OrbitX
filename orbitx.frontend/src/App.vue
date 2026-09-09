<template>
  <div style="background: #0d0e12; color: #e2e8f0; font-family: 'Courier New', monospace; padding: 30px; min-height: 100vh;">

    <h2 style="color: #38bdf8; border-bottom: 2px solid #1e293b; padding-bottom: 10px; margin-top: 0;">
      🛰️ OrbitX // Слежение за спутником
    </h2>

    <!-- Панель управления -->
    <div style="background: #15171e; padding: 20px; border: 1px solid #2a2f3a; border-radius: 6px; margin-bottom: 25px;">
      <label style="font-weight: bold; color: #94a3b8;">Введите NORAD ID спутника: </label>
      <input v-model.number="targetId"
             type="number"
             style="background: #000; color: #4ade80; border: 1px solid #3b4252; padding: 8px; width: 120px; font-family: monospace; font-size: 16px; font-weight: bold; border-radius: 4px; margin-left: 10px;" />
      <button @click="connectToSatellite"
              style="margin-left: 15px; padding: 8px 20px; background: #38bdf8; color: #000; border: none; font-weight: bold; border-radius: 4px; cursor: pointer;">
        ЗАПУСТИТЬ СЛЕЖЕНИЕ
      </button>
    </div>

    <p>Статус сокета: <span :style="{ color: statusColor }" style="font-weight: bold;">[{{ connectionStatus }}]</span></p>

    <!-- Табло -->
    <div v-if="telemetry" style="background: #000; border: 1px solid #4ade80; padding: 25px; border-radius: 6px; max-width: 550px;">
      <h3 style="color: #4ade80; margin-top: 0; margin-bottom: 15px;">📥 ДАННЫЕ ОРБИТЫ С БЭКЕНДА:</h3>
      <div style="font-size: 18px; line-height: 1.6;">
        <p style="margin: 8px 0;">🌐 Широта (Latitude): <span style="color: #fff; font-weight: bold;">{{ telemetry.latitude.toFixed(2) }}°</span></p>
        <p style="margin: 8px 0;">📐 Долгота (Longitude): <span style="color: #fff; font-weight: bold;">{{ telemetry.longitude.toFixed(2) }}°</span></p>
        <p style="margin: 8px 0;">⛰️ Высота (Altitude): <span style="color: #fff; font-weight: bold;">{{ telemetry.altitude.toFixed(2) }} км</span></p>
      </div>
    </div>

    <div v-else style="color: #64748b;">
      [SYSTEM] Ожидание старта потока вычислений...
    </div>

  </div>
</template>

<script setup lang="ts">
  import { ref, onUnmounted } from 'vue'; // Добавили onUnmounted к импортам!
  import * as signalR from '@microsoft/signalr';

  const targetId = ref(68947); // По умолчанию KUIPER-00453
  const connectionStatus = ref('ОТКЛЮЧЕН');
  const statusColor = ref('#f59e0b');

  interface SGP4DataDTO {
    latitude: number;
    longitude: number;
    altitude: number;
  }
  const telemetry = ref<SGP4DataDTO | null>(null);
  let connection: signalR.HubConnection | null = null;

  async function connectToSatellite() {
    if (connection) {
      await connection.stop();
    }

    connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/ws/satellite')
      .build();

    connection.on('ReceivePosition', (data: SGP4DataDTO) => {
      // Внедряем реактивность, чтобы Vue заметил пакет
      telemetry.value = { ...data };
    });

    try {
      await connection.start();
      connectionStatus.value = 'ПОДКЛЮЧЕНО';
      statusColor.value = '#4ade80';

      await connection.invoke('WatchSatellite', targetId.value);

    } catch (err: any) {
      connectionStatus.value = 'ОШИБКА';
      statusColor.value = '#ef4444';
      console.error(err);
    }
  }

  onUnmounted(async () => {
    if (connection) {
      await connection.invoke('UnwatchSatellite', targetId.value); // Отправляем запрос на бэк
      await connection.stop(); // Тушим поток
    }
  });
</script>
