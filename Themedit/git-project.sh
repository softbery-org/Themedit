#!/bin/sh

_VERSION=`cat .sbver`

echo ""
echo ""
echo "Geting files ..."
echo ""
git add .
sleep 3
echo ""
echo ""
echo "Commiting all files ...: Version $_VERSION"
echo ""
git commit -m "$_VERSION"
sleep 3
echo ""
echo ""
echo "Pushing repository on server GITHUB ..."
echo ""
git push
echo ""
echo ""
echo "All done ..."
echo "..."
sleep 3