# RWIS Project

## Encampment Battle Game

This repository is for creating MO (Multiplayer Online) encampment battle game.
The user will need to ready Google Spreadsheets and Google Apps Script API key for now to play the game.
In the future the Google Apps Script API key might not be necessary.


- [Issues](https://github.com/HoSuTa/RWIS_project/issues)
- [Project](https://github.com/HoSuTa/RWIS_project/projects/1)

## Map x GPS用のSDK
gitignoreに登録しているため，リポジトリには存在しない.
以下のunitypackageを入れる．
https://docs.mapbox.com/unity/maps/guides/#install-the-maps-sdk-for-unity

## System Overview

![SystemOverView](https://user-images.githubusercontent.com/63632758/146634311-9139eee5-5637-467b-b84a-431b32611f23.png)

## Git
rebaseしてコンフリクトを自分のbranchの方で確認してください．
~~その後，mergeする先のbranchにcheckoutして，
git merge --no-ff branch_name
でmergeしてください．fast-forward mergeにしないようにしましょう．~~
GitHubのサイトでプルリク送るようにしましょう，そこでrebase & mergeとか選ばずにデフォルトのmergeでok.

通常mergeはffで，コンフリクトが起こる際はno-ffになります（コンフリクト起こった時点no-ffとは言えないの方が正しいかも）．
branchのコミットの分岐をより良く残すためにno-ffをオプションとして入れる．
