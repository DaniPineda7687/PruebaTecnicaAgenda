using System;
using Newtonsoft.Json;

namespace CitasAgenda;

public class Agenda
{

    public const string HORA_INICIO_SERVICIO = "09:00";
    public const string HORA_FINAL_SERVICIO = "17:00";
    public const int DURACION_MINIMA_SERVICIO = 30;
    public const int TOTAL_MINUTOS_HORA = 60;


    public int GetTotalCitasDisponibles(string dia)
    {
        List<Cita> citas = GetCitasProgramadas().Where(c => c.Day == dia).ToList();
        citas.Add(new Cita() { Day = dia, Duration = "0", Hour = HORA_FINAL_SERVICIO });
        int totalCitasDisponibles = 0;
        string proximaCita = HORA_INICIO_SERVICIO;

        var horaFinServicioFormatted = GetHoraMinutosFromString(HORA_FINAL_SERVICIO);

        foreach (Cita cita in citas)
        {
            int tiempoTotal = GetTiempoTotalEntreHoras(proximaCita, cita.Hour);
            proximaCita = GetHoraFinalCita(cita.Hour, cita.Duration);

            var horaProximaFormatted = GetHoraMinutosFromString(proximaCita);

            if (horaProximaFormatted.Item1 == horaFinServicioFormatted.Item1)
            {
                if (horaProximaFormatted.Item2 <= horaFinServicioFormatted.Item2)
                {
                    totalCitasDisponibles += tiempoTotal / DURACION_MINIMA_SERVICIO;
                }
            }
            else
            {
                totalCitasDisponibles += tiempoTotal / DURACION_MINIMA_SERVICIO;
            }


        }

        return totalCitasDisponibles;

    }

    public (int, int) GetHoraMinutosFromString(string hora)
    {
        int horaInicioNum = int.Parse(hora.Split(":")[0].ToString());
        int minutoInicioNum = int.Parse(hora.Split(":")[1].ToString());

        return (horaInicioNum, minutoInicioNum);
    }

    public List<Cita> GetCitasProgramadas()
    {
        string rutaArchivo = "./citas.json";
        string json = File.ReadAllText(rutaArchivo);
        var horaFinServicioFormatted = GetHoraMinutosFromString(HORA_FINAL_SERVICIO);


        List<Cita> allCitas = JsonConvert.DeserializeObject<List<Cita>>(json)!;

        List<Cita> citasFormatted = new List<Cita>();


        foreach(Cita cita in allCitas)
        {
            var horaInicioFormatted = GetHoraMinutosFromString(cita.Hour);
            if(horaInicioFormatted.Item1>horaFinServicioFormatted.Item1)
            {
                continue;
            }
            if(horaInicioFormatted.Item1 == horaFinServicioFormatted.Item1)
            {
                if(horaInicioFormatted.Item2 > horaFinServicioFormatted.Item2)
                {
                    continue;
                }
            }
            citasFormatted.Add(cita);
        }


        return citasFormatted;
    }


    public int GetTiempoTotalEntreHoras(string horaInicio, string horaFin)
    {
        var horaInicioFormatted = GetHoraMinutosFromString(horaInicio);
        var horaFinFormatted = GetHoraMinutosFromString(horaFin);

        int tempNewHour = horaFinFormatted.Item1 - horaInicioFormatted.Item1;
        int tempNewMinute = horaFinFormatted.Item2 - horaInicioFormatted.Item2;

        int minutosTotalesEntreHoras = (tempNewHour * TOTAL_MINUTOS_HORA) + tempNewMinute;

        return minutosTotalesEntreHoras;
    }



    public string GetHoraFinalCita(string horaInicio, string duration)
    {
        var horaInicioFormatted = GetHoraMinutosFromString(horaInicio);

        int duracion = int.Parse(duration);

        int finalMinute = duracion + horaInicioFormatted.Item2;

        if (finalMinute >= TOTAL_MINUTOS_HORA)
        {
            int tempNewMinute = finalMinute - TOTAL_MINUTOS_HORA;
            int tempNewHour = ++horaInicioFormatted.Item1;
            return GetHoraFinalCita($"{tempNewHour}:{tempNewMinute}", "0");
        }
        else
        {
            return $"{horaInicioFormatted.Item1}:{finalMinute}";
        }
    }


}

public class Cita
{
    public string Day { get; set; } = null!;
    public string Hour { get; set; } = null!;
    public string Duration { get; set; } = null!;

}