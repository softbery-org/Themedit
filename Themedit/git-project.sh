_VERSION=`cat .sbver`
echo ""
echo "Geting files ..."
echo ""
git add .
sleep 3s
echo ""
echo ""
echo "Commiting all files ...: Version $_VERSION"
echo ""
git commit -m "Version $_VERSION"
sleep 3s
echo ""
echo ""
echo "Pushing repository on server GITHUB ..."
echo ""
git push
echo "All done ..."
echo "..."