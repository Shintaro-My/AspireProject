import SearchForm from "./searchForm"
import ShowInfo, { DataFormat } from "./showInfo"
import type { Metadata } from "next"
import "./style.scss"

export const metadata: Metadata = {
  title: "Index",
  description: "Generated by create next app",
}

const Layout = ({ children }: Readonly<{children: React.ReactNode}>) => {
  return (
    <div className="content_wrap">
      {children}
    </div>
  )
}

export default Layout