name: Build solution

on: [push]

jobs:

  build:
    name: Build on ${{ matrix.os }}
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]

    steps:
    - name: Checkout master
      uses: actions/checkout@master

    - name: Setup .NET Core
      uses: actions/setup-dotnet@v1
      with:
        version: 2.2.401
    
    - name: Build with dotnet
      run: dotnet build --configuration Release
