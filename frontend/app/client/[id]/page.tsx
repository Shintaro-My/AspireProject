import ShowPage from "../page"
import { DataFormat } from "../showInfo"

const getDataSC = async (): Promise<DataFormat[]> => {
  const apiServer = process.env['services__webapi__1']
  const weatherData: Response = await fetch(`${apiServer}/api/weatherforecast`, { cache: 'no-cache' })
  if (!weatherData.ok) {
      throw new Error('Failed to fetch data.')
  }
  const data = await weatherData.json()
  return data
}

const StatusPage = async ({ params }: { params: { id: string } }) => {
  const { id } = params
  const data = await getDataSC()
  return (
    <ShowPage initialData={data}></ShowPage>
  )
}

export default StatusPage
