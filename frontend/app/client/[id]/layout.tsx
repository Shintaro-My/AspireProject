import type { Metadata } from "next"

type Props = {
  params: { id: string }
  searchParams: { [key: string]: string | string[] | undefined }
}
 
export const generateMetadata = ({ params, searchParams }: Props): Metadata => {
  const id = params.id
  return {
    title: `ID: ${id} | Index`,
  }
}

const Layout = ({ children }: Readonly<{children: React.ReactNode}>) => {
  return <div className="content">{children}</div>
}

export default Layout