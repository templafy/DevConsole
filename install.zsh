#!/bin/zsh

realpath() {
    [[ $1 = /* ]] && echo "$1" || echo "$PWD/${1#./}"
}

SCRIPT=$(realpath "$0")
BASEDIR=$(dirname "$SCRIPT")

# add shims folder to PATH
ADD_PATH_LINE="PATH=\$PATH:$BASEDIR/shims/"

if [ -f ~/.zshrc ]; then
    grep -qxF $ADD_PATH_LINE ~/.zshrc || echo $ADD_PATH_LINE >> ~/.zshrc
else
    echo $ADD_PATH_LINE >> ~/.zshrc
fi


dev --help

echo 'Run ". ~/.zshrc" to apply to current session or restart your shell.'
