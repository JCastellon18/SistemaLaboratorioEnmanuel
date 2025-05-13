document.addEventListener("DOMContentLoaded", function () {
    fetch('/Dashboard/GetExamenesMasVendidos')
        .then(response => response.json())
        .then(data => {
            const nombres = data.map(item => item.NombreExamen);
            const cantidades = data.map(item => item.Cantidad);

            const colores = [
                '#3399FF',  // Dibujo
                '#FFCC66',  // Matemática
                '#33CC33',  // Lengua
                '#FF6600',  // Biología
                '#CCCCFF',  // Portugués
                '#FF99FF',  // Historia
                '#3333FF',  // Inglés
                '#66FF66',  // Francés
                '#FFCC00',  // Música
                '#FF3333'   // Geografía
            ];

            const ctx = document.getElementById('examenesMasVendidosChart').getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: nombres,
                    datasets: [{
                        label: 'Cantidad Vendida',
                        data: cantidades,
                        backgroundColor: colores.slice(0, nombres.length), // Aplica los colores extraídos
                        borderColor: '#000000',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    },
                    plugins: {
                        legend: {
                            display: false
                        }
                    }
                }
            });
        })
        .catch(error => console.error('Error al cargar los datos:', error));


    fetch('/Dashboard/GetOrdenesPorEstado')
        .then(response => response.json())
        .then(data => {
            const ctx = document.getElementById('ordenesPorEstadoChart').getContext('2d');

            new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: ['Órdenes Completadas', 'Órdenes Pendientes'],
                    datasets: [{
                        data: [data.Completadas, data.Pendientes],
                        backgroundColor: ['#4CAF50', '#FF5722'], // Colores para cada sección
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        })
        .catch(error => console.error('Error al cargar los datos:', error));

    fetch('/Dashboard/GetIngresosUltimos6Meses')
        .then(response => response.json())
        .then(data => {
            const meses = data.map(item => item.Mes);
            const totales = data.map(item => item.Total);

            const ctx = document.getElementById('ingresosChart6m').getContext('2d');

            new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: meses,
                    datasets: [{
                        data: totales,
                        backgroundColor: [
                            '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'
                        ],
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        })
        .catch(error => console.error('Error al cargar los datos de ingresos:', error));
    
});
