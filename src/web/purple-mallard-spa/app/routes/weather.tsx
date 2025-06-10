import { useEffect, useState } from 'react';
import { apiService, type WeatherForecast } from '../services/api-service';

export default function WeatherPage() {
  const [weatherData, setWeatherData] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchWeatherData = async () => {
      try {
        setLoading(true);
        const data = await apiService.getWeatherForecast();
        setWeatherData(data);
        setError(null);
      } catch (err) {
        console.error('Failed to fetch weather data:', err);
        setError('Failed to load weather data. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    fetchWeatherData();
  }, []);

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Weather Forecast</h1>

      {loading && (
        <div className="flex justify-center items-center p-8">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
        </div>
      )}

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {!loading && !error && (
        <div className="overflow-x-auto">
          <table className="min-w-ful">
            <thead>
              <tr>
                <th className="py-2 px-4 border-b text-left">Date</th>
                <th className="py-2 px-4 border-b text-left">Temperature (C)</th>
                <th className="py-2 px-4 border-b text-left">Temperature (F)</th>
                <th className="py-2 px-4 border-b text-left">Summary</th>
              </tr>
            </thead>
            <tbody>
              {weatherData.map((forecast, index) => (
                <tr key={index}>
                  <td className="py-2 px-4 border-b">{forecast.date}</td>
                  <td className="py-2 px-4 border-b">{forecast.temperatureC}°C</td>
                  <td className="py-2 px-4 border-b">{forecast.temperatureF}°F</td>
                  <td className="py-2 px-4 border-b">{forecast.summary}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {!loading && !error && weatherData.length === 0 && (
        <p className="text-center text-gray-500">No weather data available.</p>
      )}
    </div>
  );
}
