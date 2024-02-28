
# .NET AspireでNext.jsを使ってみる

# ディレクトリ構造

## 理想

* app
  * 実際にクライアントで見ることになるページを保管する
  * ホーム以外はページカテゴリごとにフォルダ分け
  * ホーム・ページカテゴリごとに`layout.tsx`と`page.tsx`を配置
  * そのページカテゴリでのみ使うCSS（SCSS）ファイルなども配置
* components
  * `app`内で定義するには冗長なものをこちらに
  * 特定箇所でしか使わないもののみ（汎用的なものは`elements`に）
  * フォルダ分けの際には、`app`で使われているページカテゴリや象徴的な名前で分類
* context
  * `useContext`で共通化するデータを保管する`tsx`を配置
* elements
  * 使用する場所を選ばない部品単位の`tsx`を配置
  * 部品にのみ適用するCSS（SCSS）ファイルなども配置
  * 部品ごとにフォルダ分け

## ToDo

* `app/client`内のファイルがぐちゃぐちゃなので`components`などに移動
* `components/mixin.scss`はどこに置くべきか考える


# メモ
* TestSSEにアクセスするたびに`EventStream`が増える