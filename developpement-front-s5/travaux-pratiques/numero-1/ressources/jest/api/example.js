import axios from "axios";
import fetchNationalHolidays from "./api"

const getNationalHolidays = async (region = "metropole") => {
  // Doc : https://api.gouv.fr/documentation/jours-feries
  // const listNationalHolidays = await axios
  //   .get(`https://calendrier.api.gouv.fr/jours-feries/${region}.json`)
  //   .then((response) => response.data)
  //   .catch(() => [])
  const listNationalHolidays = await fetchNationalHolidays(region)

  const res = []
  let nextDate = null
  for (const [key, value] of Object.entries(listNationalHolidays)) {
    nextDate = new Date(key)
    
    res.push({
        date: nextDate.toLocaleDateString("fr"),
        name: value
    })
  }

  return res;
};

export { getNationalHolidays };
