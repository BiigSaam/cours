<!DOCTYPE html>

<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Exemple graphique courbe avec requête asynchrone chartjs</title>

    <script src="https://cdn.jsdelivr.net/npm/chart.js" defer></script>
    <script src="<?php echo $_SERVER['REQUEST_URI']; ?>/script.js" defer></script>
  </head>
  <body>
    <div>
      <canvas id="myChart"></canvas>
    </div>
  </body>
</html>
